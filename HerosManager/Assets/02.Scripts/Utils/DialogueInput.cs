using UnityEngine;

/// <summary>Only deliberate dialogue confirmation advances a line, at most once per frame.</summary>
public static class DialogueInput
{
    static int consumedFrame = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset() => consumedFrame = -1;

    public static void BlockCurrentFrame() => consumedFrame = Time.frameCount;

    public static bool ConsumeAdvance()
    {
        if (consumedFrame == Time.frameCount || !Application.isFocused) return false;
        bool pressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
            (GamepadInput.Pressed(GamepadButton.Confirm) && !GamepadInput.GameplayBlockedThisFrame);
        if (!pressed) return false;
        consumedFrame = Time.frameCount;
        GamepadInput.BlockGameplayThisFrame();
        return true;
    }
}
