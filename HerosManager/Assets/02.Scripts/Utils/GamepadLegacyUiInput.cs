using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Keep legacy keyboard/mouse UI, excluding joystick axes/buttons already handled by XInput.</summary>
public sealed class GamepadLegacyUiInput : BaseInput
{
    public override float GetAxisRaw(string axisName)
    {
        if (GamepadUiController.DirectGameplay) return 0;
        if (axisName == "Horizontal") return (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? 1 : 0);
        if (axisName == "Vertical") return (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? 1 : 0);
        return base.GetAxisRaw(axisName);
    }
    public override bool GetButtonDown(string buttonName)
    {
        if (GamepadUiController.DirectGameplay) return false;
        if (buttonName == "Submit")
        {
            if (GamepadInput.GameplayBlockedThisFrame || Object.FindFirstObjectByType<IntroManager>() != null ||
                (TutorialMgr.IsCreateInstance() && TutorialMgr.Instance != null && TutorialMgr.Instance.isDialog) ||
                (InGameMgr.IsCreateInstance() && InGameMgr.Instance != null && InGameMgr.Instance.state == State.Cutscene))
                return false;
            return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space);
        }
        if (buttonName == "Cancel") return Input.GetKeyDown(KeyCode.Escape);
        return base.GetButtonDown(buttonName);
    }
}
