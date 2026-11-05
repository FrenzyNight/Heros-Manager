using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Call from Play validation. All input/motors are virtual; restores the production source.</summary>
public static class GamepadSceneValidation
{
    public static IEnumerator RunAsync(Action completed = null)
    {
        ValidationSaveSession.RequireActive();
        if (!Application.isPlaying) throw new InvalidOperationException("GamepadSceneValidation requires Play mode.");
        var source = new VirtualPad();
        IGamepadSource previousSource = GamepadInput.SetSourceForTests(source);
        bool previousRumble = GamepadInput.RumbleEnabled;
        EventSystem events = EventSystem.current;
        GameObject ownedEvents = null;
        GameObject previousSelection = events != null ? events.currentSelectedGameObject : null;
        var overlay = new GameObject("Gamepad UI validation", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
        var controllerHost = new GameObject("Gamepad UI validation controller");
        try
        {
            if (events == null)
            {
                ownedEvents = new GameObject("Gamepad validation events", typeof(EventSystem), typeof(StandaloneInputModule));
                events = ownedEvents.GetComponent<EventSystem>();
            }
            events.SetSelectedGameObject(null);
            var canvas = overlay.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767;
            var background = new GameObject("Block other menus", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(overlay.transform, false);
            var backRect = background.GetComponent<RectTransform>();
            backRect.anchorMin = Vector2.zero; backRect.anchorMax = Vector2.one;
            backRect.offsetMin = backRect.offsetMax = Vector2.zero;
            Button first = Create<Button>(overlay.transform, "First", new Vector2(-0.25f, 0.18f));
            Button second = Create<Button>(overlay.transform, "Second", new Vector2(0.25f, 0.18f));
            Slider slider = Create<Slider>(overlay.transform, "Volume", new Vector2(0, -0.18f));
            slider.minValue = 0; slider.maxValue = 1; slider.value = 0.5f;
            int submits = 0;
            first.onClick.AddListener(() => submits++);
            var controller = controllerHost.AddComponent<GamepadUiController>();
            controller.enabled = false;
            // GraphicRaycaster excludes depth=-1 graphics. Give new overlay graphics a real render/update pass.
            yield return null;
            yield return null;
            Canvas.ForceUpdateCanvases();

            Step(source, controller, GamepadButton.None);
            string diagnostic = Diagnose(events, first, second, slider);
            Debug.Log("GAMEPAD UI DIAGNOSTICS: " + diagnostic);
            Require(events.currentSelectedGameObject == first.gameObject, "Auto focus chooses the first visible button. " + diagnostic);
            Step(source, controller, GamepadButton.Confirm);
            Require(GamepadInput.GameplayBlockedThisFrame, "UI submit consumes the gameplay frame.");
            Step(source, controller, GamepadButton.Confirm);
            Require(submits == 1, "Holding A submits exactly once.");
            Step(source, controller, GamepadButton.None);
            Step(source, controller, GamepadButton.Right);
            Require(events.currentSelectedGameObject == second.gameObject, "Right navigation reaches the next visible button.");
            Step(source, controller, GamepadButton.None);
            Step(source, controller, GamepadButton.Down);
            Require(events.currentSelectedGameObject == slider.gameObject, "Down navigation reaches the slider.");
            Step(source, controller, GamepadButton.None);
            Step(source, controller, GamepadButton.Right);
            Require(slider.value > 0.5f, "Slider right input changes its value.");
            Require(events.GetComponent<StandaloneInputModule>() == null ||
                events.GetComponent<StandaloneInputModule>().inputOverride is GamepadLegacyUiInput,
                "The legacy module excludes duplicate joystick Submit/axis input.");

            GamepadInput.RumbleEnabled = true;
            GamepadInput.Pulse(0.4f, 0.6f, 0.02f);
            if (Time.timeScale > 0f)
            {
                Require(source.Low > 0 && source.High > 0, "Virtual vibration receives both motor amplitudes.");
                GamepadInput.AdvanceFrameForTests(0.03f);
                Require(source.Low == 0 && source.High == 0, "Expired pulses stop both motors.");
            }
            if (InGameMgr.IsCreateInstance() && InGameMgr.Instance != null && GamepadUiController.GameplayInputAllowed)
            {
                var game = InGameMgr.Instance;
                State previousState = game.state;
                try
                {
                    game.state = State.MiniGame;
                    events.SetSelectedGameObject(first.gameObject);
                    source.Frame = new GamepadFrame { Connected = true };
                    GamepadInput.AdvanceFrameForTests(0.016f);
                    source.Frame = new GamepadFrame { Connected = true, Buttons = GamepadButton.Confirm };
                    GamepadInput.AdvanceFrameForTests(0.016f);
                    controller.ProcessFrameForTests();
                    Require(submits == 1, "A used by minigames must not submit a selected menu button.");
                    Debug.Log("GAMEPAD MINIGAME/UI ISOLATION VALIDATION PASSED");
                }
                finally { game.state = previousState; }
            }
            ValidateCampMovement(source);
            Debug.Log("GAMEPAD UI VALIDATION PASSED (virtual input and motors)");
        }
        finally
        {
            GamepadInput.SetSourceForTests(previousSource);
            GamepadInput.RumbleEnabled = previousRumble;
            if (events != null) events.SetSelectedGameObject(previousSelection);
            UnityEngine.Object.DestroyImmediate(controllerHost);
            UnityEngine.Object.DestroyImmediate(overlay);
            if (ownedEvents != null) UnityEngine.Object.DestroyImmediate(ownedEvents);
        }
        completed?.Invoke();
    }

    private static void ValidateCampMovement(VirtualPad source)
    {
        if (!InGameMgr.IsCreateInstance() || InGameMgr.Instance == null) return;
        var game = InGameMgr.Instance;
        State previousState = game.state;
        float previousTimeScale = Time.timeScale;
        var player = UnityEngine.Object.FindFirstObjectByType<PlayerCtrl>();
        Require(player != null && Camera.main != null, "Camp movement requires the real player and scene camera.");
        var sceneAgent = player.GetComponent<NavMeshAgent>();
        Require(sceneAgent != null && sceneAgent.isOnNavMesh, "The camp player must be on its baked NavMesh.");
        var actor = new GameObject("Gamepad navigation validation");
        actor.SetActive(false);
        actor.transform.position = sceneAgent.nextPosition;
        var agent = actor.AddComponent<NavMeshAgent>();
        agent.agentTypeID = sceneAgent.agentTypeID;
        agent.areaMask = sceneAgent.areaMask;
        agent.radius = sceneAgent.radius;
        agent.height = sceneAgent.height;
        agent.speed = 7f;
        agent.stoppingDistance = sceneAgent.stoppingDistance;
        TutorialMgr tutorial = TutorialMgr.IsCreateInstance() ? TutorialMgr.Instance : null;
        bool previousTutorial = tutorial != null && tutorial.isTutorial;
        try
        {
            game.state = State.Camp;
            Time.timeScale = 1f;
            if (tutorial != null) tutorial.isTutorial = false;
            actor.SetActive(true);
            var control = actor.AddComponent<PlayerCtrl>();
            control.enabled = false;
            Require(agent.isOnNavMesh, "The isolated navigation actor attaches to the real camp NavMesh.");
            // All steps are synchronous: the real player never receives the virtual right-stick input.
            SetMovementFrame(source, Vector2.zero);
            Require(GamepadUiController.GameplayInputAllowed, "Close campaign modals before validating camp movement.");
            Vector2 direction = Vector2.zero;
            for (int index = 0; index < 8; index++)
            {
                float angle = index * Mathf.PI / 4f;
                SetMovementFrame(source, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
                control.ProcessGamepadMovementForTests();
                if (control.IsGamepadMoving) { direction = source.Frame.Aim; break; }
            }
            Require(direction != Vector2.zero && agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete,
                "RS assigns a complete path on the actual camp NavMesh.");
            Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
            if (cameraForward.sqrMagnitude < 0.001f)
                cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.up, Vector3.up);
            cameraForward.Normalize();
            Vector3 expected = Vector3.Cross(Vector3.up, cameraForward) * direction.x + cameraForward * direction.y;
            Require(Vector3.Dot(agent.destination - agent.nextPosition, expected) > 0f,
                "The navigation destination follows the camera-relative right-stick direction.");
            Vector3 destination = agent.destination;

            SetMovementFrame(source, Vector2.zero);
            control.ProcessGamepadMovementForTests();
            Require(!control.IsGamepadMoving && !agent.hasPath && agent.velocity == Vector3.zero, "Releasing RS immediately stops pad movement.");
            source.Frame = new GamepadFrame { Connected = true, Move = Vector2.right };
            GamepadInput.AdvanceFrameForTests(0.016f);
            control.ProcessGamepadMovementForTests();
            Require(!control.IsGamepadMoving && !agent.hasPath, "LS remains reserved for UI selection in camp.");

            SetMovementFrame(source, direction);
            control.ProcessGamepadMovementForTests();
            Require(control.IsGamepadMoving, "RS restarts after release.");
            game.state = State.Gathering;
            control.ProcessGamepadMovementForTests();
            Require(!control.IsGamepadMoving && !agent.hasPath, "Leaving Camp cancels pad navigation.");
            game.state = State.Camp;
            control.ProcessGamepadMovementForTests();
            Require(control.IsGamepadMoving, "RS resumes after returning to Camp.");
            GamepadInput.BlockGameplayThisFrame();
            control.ProcessGamepadMovementForTests();
            Require(!control.IsGamepadMoving && !agent.hasPath, "A UI-consumed frame cancels pad navigation.");

            SetMovementFrame(source, direction);
            control.ProcessGamepadMovementForTests();
            Require(control.IsGamepadMoving, "RS resumes on an unconsumed frame.");
            Time.timeScale = 0f;
            control.ProcessGamepadMovementForTests();
            Require(!control.IsGamepadMoving && !agent.hasPath, "Pause cancels pad navigation.");
            Time.timeScale = 1f;
            control.ProcessGamepadMovementForTests();
            Require(control.IsGamepadMoving, "RS resumes after unpausing.");
            if (tutorial != null)
            {
                tutorial.isTutorial = true;
                control.ProcessGamepadMovementForTests();
                Require(!control.IsGamepadMoving && !agent.hasPath, "Tutorial modals cancel pad navigation.");
                tutorial.isTutorial = false;
            }
            control.ProcessGamepadMovementForTests();
            Require(control.IsGamepadMoving, "RS resumes after the tutorial closes.");
            GamepadInput.AdvanceFrameForTests(0.016f, false);
            control.ProcessGamepadMovementForTests();
            Require(!control.IsGamepadMoving && !agent.hasPath, "Focus loss cancels pad navigation.");
            SetMovementFrame(source, Vector2.zero);
            SetMovementFrame(source, direction);
            control.ProcessGamepadMovementForTests();
            Require(control.IsGamepadMoving, "RS resumes after focus and neutral input return.");
            source.Frame = default;
            GamepadInput.AdvanceFrameForTests(0.016f);
            control.ProcessGamepadMovementForTests();
            Require(!control.IsGamepadMoving && !agent.hasPath, "Disconnect cancels pad navigation.");

            // A non-pad destination must survive idle/disconnected pad polling.
            var mousePath = new NavMeshPath();
            Require(agent.CalculatePath(destination, mousePath) && agent.SetPath(mousePath), "Restore a non-pad navigation path.");
            control.ProcessGamepadMovementForTests();
            Require(agent.hasPath, "Idle gamepad polling preserves a mouse-owned destination.");
            Debug.Log("GAMEPAD CAMP NAVMESH VALIDATION PASSED (camera-relative path, release, LS isolation, state/UI/pause/tutorial/focus/disconnect gates, mouse path preservation)");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(actor);
            game.state = previousState;
            Time.timeScale = previousTimeScale;
            if (tutorial != null) tutorial.isTutorial = previousTutorial;
            SetMovementFrame(source, Vector2.zero);
        }
    }

    private static void SetMovementFrame(VirtualPad source, Vector2 aim)
    {
        source.Frame = new GamepadFrame { Connected = true, Aim = aim };
        GamepadInput.AdvanceFrameForTests(0.016f);
    }

    private static T Create<T>(Transform parent, string name, Vector2 position) where T : Selectable
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(Screen.width * 0.25f, Screen.height * 0.12f);
        rect.anchoredPosition = new Vector2(position.x * Screen.width, position.y * Screen.height);
        var selectable = obj.AddComponent<T>();
        selectable.targetGraphic = obj.GetComponent<Image>();
        return selectable;
    }

    private static string Diagnose(EventSystem events, params Selectable[] controls)
    {
        string result = $"screen={Screen.width}x{Screen.height}, selected={(events.currentSelectedGameObject != null ? events.currentSelectedGameObject.name : "<null>")}";
        var hits = new List<RaycastResult>();
        foreach (var control in controls)
        {
            var rect = (RectTransform)control.transform;
            Vector2 point = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
            hits.Clear();
            events.RaycastAll(new PointerEventData(events) { position = point }, hits);
            result += $"; {control.name}: depth={control.targetGraphic.depth}, center={point}, top={(hits.Count > 0 ? hits[0].gameObject.name : "<none>")}, hits={hits.Count}";
        }
        return result;
    }

    private static void Step(VirtualPad source, GamepadUiController controller, GamepadButton buttons)
    {
        source.Frame = new GamepadFrame { Connected = true, Buttons = buttons };
        GamepadInput.AdvanceFrameForTests(0.016f);
        controller.ProcessVisibleUiForTests();
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    private sealed class VirtualPad : IGamepadSource
    {
        public GamepadFrame Frame;
        public float Low, High;
        public GamepadFrame Read() => Frame;
        public void SetVibration(float low, float high) { Low = low; High = high; }
    }
}
