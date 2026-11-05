using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(-9000)]
public sealed class GamepadUiController : MonoBehaviour
{
    private readonly List<Selectable> candidates = new List<Selectable>();
    private readonly List<RaycastResult> hits = new List<RaycastResult>();
    private float nextMove;
    private Vector2 lastDirection;
    private Text hints;
    private RectTransform hintBackground;
    private GameObject highlight;
    private RectTransform highlightRect;
    private EventSystem configuredSystem;
    private bool koreanHints;
    private float nextFontCheck;

    public static bool GameplayInputAllowed
    {
        get
        {
            if (!GamepadInput.Focused || Time.timeScale <= 0f || GamepadInput.GameplayBlockedThisFrame) return false;
            if (TutorialMgr.IsCreateInstance() && TutorialMgr.Instance != null && TutorialMgr.Instance.isTutorial) return false;
            if (MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance != null && MeadowCampaign.Instance.IsModalOpen) return false;
            return true;
        }
    }

    public static bool MiniGameInputAllowed => GameplayInputAllowed && InGameMgr.IsCreateInstance() &&
        InGameMgr.Instance != null && InGameMgr.Instance.state == State.MiniGame;

    public static bool DirectGameplay
    {
        get
        {
            if (!GameplayInputAllowed || !InGameMgr.IsCreateInstance() || InGameMgr.Instance == null) return false;
            if (InGameMgr.Instance.state == State.MiniGame) return true;
            if (InGameMgr.Instance.state != State.Laundry) return false;
            var laundry = Object.FindFirstObjectByType<LaundryInteractionMgr>();
            return laundry != null && ((laundry.laundryState == 2 && laundry.l2Count < laundry.l2CountNum) ||
                (laundry.laundryState == 3 && laundry.l3CanPress));
        }
    }

    private void Update()
    {
        ConfigureLegacyModule();
        if (GamepadInput.Pressed(GamepadButton.Back) || GamepadInput.Pressed(GamepadButton.Pause))
        {
            GamepadInput.BlockGameplayThisFrame();
            GamepadInput.StopRumble();
            SetHighlight(null);
            return;
        }
        if (!GamepadInput.Connected || !GamepadInput.Focused)
        {
            if (hints != null) hints.gameObject.SetActive(false);
            if (hintBackground != null) hintBackground.gameObject.SetActive(false);
            SetHighlight(null);
            return;
        }
        UpdateHints();
        if (MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance != null && MeadowCampaign.Instance.IsModalOpen)
            MeadowCampaign.Instance.ScrollReport(GamepadInput.Aim.y);
        if (DirectGameplay)
        {
            SetHighlight(null);
            return;
        }
        // These dialogue screens own A directly; submitting their visible skip/window controls would double advance.
        if (Object.FindFirstObjectByType<IntroManager>() != null ||
            (InGameMgr.IsCreateInstance() && InGameMgr.Instance.state == State.Cutscene) ||
            (TutorialMgr.IsCreateInstance() && TutorialMgr.Instance != null && TutorialMgr.Instance.isDialog) ||
            (InGameMgr.IsCreateInstance() && InGameMgr.Instance != null && InGameMgr.Instance.state == State.Cutscene))
        {
            SetHighlight(null);
            return;
        }
        var title = Object.FindFirstObjectByType<TitleManager>();
        if (title != null && !title.MenuReady) { SetHighlight(null); return; }
        ProcessVisibleUi();
    }

    public void ProcessFrameForTests() => Update();

    // Exercises the same navigation/submit path independently of the loaded title/game scene.
    public void ProcessVisibleUiForTests()
    {
        ConfigureLegacyModule();
        ProcessVisibleUi();
    }

    private void ProcessVisibleUi()
    {
        EventSystem events = EventSystem.current;
        if (events == null) return;
        CollectVisible(events);
        if (candidates.Count == 0) { SetHighlight(null); return; }
        Selectable selected = events.currentSelectedGameObject == null ? null : events.currentSelectedGameObject.GetComponent<Selectable>();
        if (selected == null || !candidates.Contains(selected))
        {
            selected = candidates[0];
            events.SetSelectedGameObject(selected.gameObject);
        }
        Vector2 direction = GamepadInput.Move;
        if (direction.sqrMagnitude > 0.2f && (lastDirection == Vector2.zero || Vector2.Dot(direction, lastDirection) < 0.5f || Time.unscaledTime >= nextMove))
        {
            GamepadInput.BlockGameplayThisFrame();
            nextMove = Time.unscaledTime + (lastDirection == Vector2.zero ? 0.4f : 0.13f);
            if (selected is Slider slider && Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                slider.value += Mathf.Sign(direction.x) * (slider.wholeNumbers ? 1f : (slider.maxValue - slider.minValue) * 0.05f);
            else
            {
                var next = FindNext(selected, direction);
                if (next != null) { selected = next; events.SetSelectedGameObject(next.gameObject); }
            }
        }
        lastDirection = direction.sqrMagnitude > 0.2f ? direction.normalized : Vector2.zero;
        SetHighlight(selected);
        if (GamepadInput.Pressed(GamepadButton.Confirm))
        {
            GamepadInput.BlockGameplayThisFrame();
            ExecuteEvents.Execute(selected.gameObject, new BaseEventData(events), ExecuteEvents.submitHandler);
        }
    }

    private void ConfigureLegacyModule()
    {
        var events = EventSystem.current;
        if (events == null || configuredSystem == events) return;
        configuredSystem = events;
        var module = events.GetComponent<StandaloneInputModule>();
        if (module != null && module.inputOverride == null)
            module.inputOverride = events.gameObject.AddComponent<GamepadLegacyUiInput>();
    }

    private void CollectVisible(EventSystem events)
    {
        candidates.Clear();
        var laundry = Object.FindFirstObjectByType<LaundryInteractionMgr>();
        bool selectingStain = GameplayInputAllowed && laundry != null && laundry.laundryUIPanel.activeInHierarchy &&
            laundry.laundryState == 1 && !laundry.laundryUIButton.interactable;
        foreach (var selectable in Selectable.allSelectablesArray)
        {
            if (selectable == null || !selectable.IsActive() || !selectable.IsInteractable()) continue;
            if (selectingStain && selectable.GetComponent<LaundryGrimeScript>() == null) continue;
            var rect = selectable.transform as RectTransform;
            var canvas = selectable.GetComponentInParent<Canvas>();
            if (rect == null || canvas == null) continue;
            if (HasVisiblePoint(events, selectable, rect, canvas))
                candidates.Add(selectable);
        }
        // Stable first focus in visual reading order, rather than scene component registration order.
        candidates.Sort((a, b) =>
        {
            Vector2 first = ScreenCenter(a), second = ScreenCenter(b);
            int y = second.y.CompareTo(first.y);
            return y != 0 ? y : first.x.CompareTo(second.x);
        });
    }

    private static Vector2 ScreenCenter(Selectable selectable)
    {
        var rect = (RectTransform)selectable.transform;
        var canvas = selectable.GetComponentInParent<Canvas>();
        return RectTransformUtility.WorldToScreenPoint(EventCamera(canvas),
            rect.TransformPoint(rect.rect.center));
    }

    private static Camera EventCamera(Canvas canvas) => canvas.renderMode == RenderMode.ScreenSpaceOverlay
        ? null : canvas.worldCamera != null ? canvas.worldCamera : Camera.main;

    private bool HasVisiblePoint(EventSystem events, Selectable selectable, RectTransform rect, Canvas canvas)
    {
        // A partially clipped/covered control can still be used at an exposed point.
        // Never accept a control through a modal's raycast blocker.
        for (int index = 0; index < 5; index++)
        {
            Vector2 local = rect.rect.center;
            if (index > 0)
                local += new Vector2(index < 3 ? -0.3f : 0.3f, index % 2 == 0 ? -0.3f : 0.3f) * rect.rect.size;
            Vector2 point = RectTransformUtility.WorldToScreenPoint(EventCamera(canvas),
                rect.TransformPoint(local));
            if (point.x < 0 || point.x >= Screen.width || point.y < 0 || point.y >= Screen.height) continue;
            hits.Clear();
            events.RaycastAll(new PointerEventData(events) { position = point }, hits);
            if (hits.Count > 0 && (hits[0].gameObject == selectable.gameObject || hits[0].gameObject.transform.IsChildOf(selectable.transform)))
                return true;
        }
        return false;
    }

    private Selectable FindNext(Selectable current, Vector2 direction)
    {
        Selectable best = null;
        float bestScore = float.PositiveInfinity;
        direction = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? new Vector2(Mathf.Sign(direction.x), 0) : new Vector2(0, Mathf.Sign(direction.y));
        foreach (var candidate in candidates)
        {
            if (candidate == current) continue;
            // Camp controls also live on world-space canvases; compare everything in screen pixels.
            Vector2 offset = ScreenCenter(candidate) - ScreenCenter(current);
            float forward = Vector2.Dot(offset, direction);
            if (forward <= 1f) continue;
            float score = offset.sqrMagnitude / forward;
            if (score < bestScore) { bestScore = score; best = candidate; }
        }
        return best;
    }

    private void UpdateHints()
    {
        if (hints == null)
        {
            var canvasObject = new GameObject("Gamepad help", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32000;
            var background = new GameObject("Controls background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(canvasObject.transform, false);
            hintBackground = background.GetComponent<RectTransform>();
            hintBackground.anchorMin = Vector2.zero; hintBackground.anchorMax = new Vector2(1, 0);
            hintBackground.pivot = new Vector2(0.5f, 0);
            background.GetComponent<Image>().color = GameUiStyle.Surface;
            background.GetComponent<Image>().raycastTarget = false;
            var textObject = new GameObject("Controls", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(canvasObject.transform, false);
            hints = textObject.GetComponent<Text>();
            hints.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            GameUiStyle.TextStyle(hints, 20, 20, GameUiStyle.Paper);
            hints.alignment = TextAnchor.MiddleCenter;
            hints.raycastTarget = false;
            var rect = hints.rectTransform;
            rect.anchorMin = new Vector2(0, 0); rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0); rect.sizeDelta = new Vector2(-48, 64);
            rect.anchoredPosition = new Vector2(0, 8);
            highlight = new GameObject("Focus", typeof(RectTransform), typeof(Image));
            highlight.transform.SetParent(canvasObject.transform, false);
            highlightRect = highlight.GetComponent<RectTransform>();
            highlight.GetComponent<Image>().color = new Color(1, 0.85f, 0.1f, 0.06f);
            highlight.GetComponent<Image>().raycastTarget = false;
            AddFocusEdge("Top", new Vector2(0, 1), Vector2.one, new Vector2(0, 3));
            AddFocusEdge("Bottom", Vector2.zero, new Vector2(1, 0), new Vector2(0, 3));
            AddFocusEdge("Left", Vector2.zero, new Vector2(0, 1), new Vector2(3, 0));
            AddFocusEdge("Right", new Vector2(1, 0), Vector2.one, new Vector2(3, 0));
            highlight.SetActive(false);
        }
        hints.gameObject.SetActive(true);
        hintBackground.gameObject.SetActive(true);
        if (!koreanHints && Time.unscaledTime >= nextFontCheck)
        {
            nextFontCheck = Time.unscaledTime + 1f;
            foreach (var sceneText in Object.FindObjectsByType<Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                if (sceneText != hints && sceneText.font != null && sceneText.font.HasCharacter('가'))
                {
                    hints.font = sceneText.font;
                    koreanHints = true;
                    break;
                }
        }
        hints.text = MiniGameInputAllowed
            ? "Xbox: LS / D-pad move & chop | A jump | X hold: slide | RS aim / RT shoot | LB fish | RS left/right water | B finish | Start pause"
            : "Xbox: LS / D-pad select | A confirm | B back | Start pause | Sliders: left/right";
        if (InGameMgr.IsCreateInstance() && InGameMgr.Instance != null && InGameMgr.Instance.state == State.Camp)
            hints.text = "Xbox Camp: RS move | LS / D-pad select | A nearby action / confirm | B back | Start pause";
        if (InGameMgr.IsCreateInstance() && InGameMgr.Instance != null && InGameMgr.Instance.state == State.Laundry)
            hints.text = "Xbox Laundry: LS / D-pad select stain + A clean | Rinse: LB / RB alternate | Dry: hold displayed button | B back";
        if (koreanHints)
        {
            hints.text = MiniGameInputAllowed
                ? "Xbox: LS/십자키 이동·벌목 | A 점프 · X 누르면 숙이기 | RS 조준 · RT 발사 | LB 낚시 | RS 좌우 물통 | B 마치기 · Start 일시정지"
                : "Xbox: LS/십자키 선택 | A 확인 · B 뒤로 · Start 일시정지 | 슬라이더: 좌우";
            if (InGameMgr.IsCreateInstance() && InGameMgr.Instance != null && InGameMgr.Instance.state == State.Camp)
                hints.text = "Xbox 캠프: RS 이동 | LS/십자키 선택 | A 근처 시설 이용·확인 | B 뒤로 · Start 일시정지";
            if (InGameMgr.IsCreateInstance() && InGameMgr.Instance != null && InGameMgr.Instance.state == State.Laundry)
                hints.text = "Xbox 빨래: LS/십자키 얼룩 선택 · A 닦기 | 헹구기: LB/RB 교대로 | 말리기: 표시된 버튼 누르기 | B 뒤로";
        }
        if (MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance != null && MeadowCampaign.Instance.IsModalOpen)
            hints.text = koreanHints
                ? "LS/십자키 버튼 선택 · A 확인 · RS 위/아래 본문 스크롤 · B 뒤로"
                : "LS / D-pad select · A confirm · RS up/down scroll report · B back";
        if (Object.FindFirstObjectByType<IntroManager>() != null ||
            (TutorialMgr.IsCreateInstance() && TutorialMgr.Instance != null && TutorialMgr.Instance.isDialog) ||
            (InGameMgr.IsCreateInstance() && InGameMgr.Instance != null && InGameMgr.Instance.state == State.Cutscene))
            hints.text = koreanHints ? "A 다음 대사" : "A advance dialogue";
        hints.fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height / 54f), 15, 22);
        float hintHeight = Mathf.Clamp(hints.preferredHeight + 12f, 40f, Mathf.Max(40f, Screen.height * 0.24f));
        hints.rectTransform.sizeDelta = new Vector2(-48, hintHeight);
        hintBackground.sizeDelta = new Vector2(0, hintHeight + 16f);
    }

    private void AddFocusEdge(string name, Vector2 minimum, Vector2 maximum, Vector2 size)
    {
        var edge = new GameObject(name, typeof(RectTransform), typeof(Image));
        edge.transform.SetParent(highlight.transform, false);
        var rect = edge.GetComponent<RectTransform>();
        rect.anchorMin = minimum; rect.anchorMax = maximum;
        rect.sizeDelta = size;
        edge.GetComponent<Image>().color = GameUiStyle.Accent;
        edge.GetComponent<Image>().raycastTarget = false;
    }

    private void SetHighlight(Selectable selected)
    {
        if (highlight == null) return;
        highlight.SetActive(selected != null);
        if (selected == null) return;
        var rect = selected.transform as RectTransform;
        var canvas = selected.GetComponentInParent<Canvas>();
        var corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        var camera = EventCamera(canvas);
        Vector2 bottom = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
        Vector2 top = bottom;
        for (int i = 1; i < corners.Length; i++)
        {
            Vector2 point = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
            bottom = Vector2.Min(bottom, point);
            top = Vector2.Max(top, point);
        }
        highlightRect.position = (bottom + top) * 0.5f;
        highlightRect.sizeDelta = top - bottom + new Vector2(8, 8);
    }
}
