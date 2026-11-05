using System;
using UnityEngine;

[Flags]
public enum GamepadButton : uint
{
    None = 0, Up = 1, Down = 2, Left = 4, Right = 8,
    Pause = 16, View = 32, LeftStick = 64, RightStick = 128,
    LeftShoulder = 256, RightShoulder = 512,
    Confirm = 4096, Back = 8192, X = 16384, Y = 32768,
    LeftTrigger = 65536, RightTrigger = 131072
}

public struct GamepadFrame
{
    public bool Connected;
    public GamepadButton Buttons;
    public Vector2 Move, Aim;
    public float LeftTrigger, RightTrigger;
}

/// <summary>Sources can be replaced by deterministic virtual frames without native devices.</summary>
public interface IGamepadSource
{
    GamepadFrame Read();
    void SetVibration(float low, float high);
}

public sealed class GamepadState
{
    public GamepadFrame Current { get; private set; }
    public GamepadButton Previous { get; private set; }
    private bool armed;

    public void Step(GamepadFrame sample, bool focused)
    {
        Previous = Current.Buttons;
        if (!focused || !sample.Connected)
        {
            Current = default;
            Previous = GamepadButton.None;
            armed = false;
            return;
        }
        sample.Move = DeadZone(sample.Move, 0.22f);
        sample.Aim = DeadZone(sample.Aim, 0.24f);
        if (sample.LeftTrigger > 0.25f) sample.Buttons |= GamepadButton.LeftTrigger;
        if (sample.RightTrigger > 0.25f) sample.Buttons |= GamepadButton.RightTrigger;
        if (sample.Move.y > 0.5f) sample.Buttons |= GamepadButton.Up;
        if (sample.Move.y < -0.5f) sample.Buttons |= GamepadButton.Down;
        if (sample.Move.x < -0.5f) sample.Buttons |= GamepadButton.Left;
        if (sample.Move.x > 0.5f) sample.Buttons |= GamepadButton.Right;
        Vector2 dpad = new Vector2(
            ((sample.Buttons & GamepadButton.Right) != 0 ? 1 : 0) - ((sample.Buttons & GamepadButton.Left) != 0 ? 1 : 0),
            ((sample.Buttons & GamepadButton.Up) != 0 ? 1 : 0) - ((sample.Buttons & GamepadButton.Down) != 0 ? 1 : 0));
        if (sample.Move == Vector2.zero) sample.Move = dpad.normalized;
        // Reconnection/focus regain requires neutral input: held A must not confirm a new screen.
        if (!armed)
        {
            armed = sample.Buttons == GamepadButton.None && sample.Move == Vector2.zero && sample.Aim == Vector2.zero;
            sample.Buttons = GamepadButton.None;
            sample.Move = sample.Aim = Vector2.zero;
            Previous = GamepadButton.None;
        }
        Current = sample;
    }

    public bool Held(GamepadButton button) => (Current.Buttons & button) != 0;
    public bool Pressed(GamepadButton button) => Held(button) && (Previous & button) == 0;
    public bool Released(GamepadButton button) => !Held(button) && (Previous & button) != 0;

    public static Vector2 DeadZone(Vector2 value, float deadZone)
    {
        if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsInfinity(value.x) || float.IsInfinity(value.y))
            return Vector2.zero;
        float length = value.magnitude;
        if (length <= deadZone) return Vector2.zero;
        return value.normalized * Mathf.Clamp01((length - deadZone) / (1f - deadZone));
    }
}

public static class GamepadInput
{
    private static GamepadState state = new GamepadState();
    private static IGamepadSource source;
    private static float rumbleRemaining;
    private static int blockedFrame = -1;
    private static bool rumbleEnabled = true;
    private static readonly KeyCode[] legacyKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
    public static bool Connected => state.Current.Connected;
    public static bool Focused { get; private set; }
    public static Vector2 Move => state.Current.Move;
    public static Vector2 Aim => state.Current.Aim;
    public static bool Held(GamepadButton button) => state.Held(button);
    public static bool Pressed(GamepadButton button) => state.Pressed(button);
    public static bool Released(GamepadButton button) => state.Released(button);
    public static bool KeyboardOrMousePressed
    {
        get
        {
            if (!Input.anyKeyDown) return false;
            foreach (KeyCode key in legacyKeys)
                if (key > KeyCode.None && key < KeyCode.JoystickButton0 && Input.GetKeyDown(key)) return true;
            return false;
        }
    }
    public static bool GameplayBlockedThisFrame => blockedFrame == Time.frameCount;
    public static bool RumbleEnabled
    {
        get => rumbleEnabled;
        set { rumbleEnabled = value; if (!value) StopRumble(); }
    }
    public static void BlockGameplayThisFrame() => blockedFrame = Time.frameCount;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        StopRumble();
        source = null;
        state = new GamepadState();
        blockedFrame = -1;
        rumbleEnabled = true;
        Focused = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        source = new WindowsGamepadSource();
        var host = new GameObject("Gamepad input");
        UnityEngine.Object.DontDestroyOnLoad(host);
        host.AddComponent<GamepadInputDriver>();
        host.AddComponent<GamepadUiController>();
    }

    public static IGamepadSource SetSourceForTests(IGamepadSource replacement)
    {
        StopRumble();
        IGamepadSource previous = source;
        source = replacement;
        state = new GamepadState();
        return previous;
    }

    public static void AdvanceFrameForTests(float deltaTime, bool focused = true)
    {
        blockedFrame = -1;
        Poll(deltaTime, focused);
    }

    internal static void Poll(float deltaTime, bool focused)
    {
        Focused = focused;
        state.Step(source != null ? source.Read() : default, focused);
        if (!focused || !Connected || Time.timeScale <= 0f)
            StopRumble();
        else if (rumbleRemaining > 0f)
        {
            rumbleRemaining -= Mathf.Max(0f, deltaTime);
            if (rumbleRemaining <= 0f) StopRumble();
        }
    }

    public static void Pulse(float low, float high, float seconds)
    {
        if (!rumbleEnabled || !Connected || !Focused || Time.timeScale <= 0f || source == null) return;
        if (float.IsNaN(low) || float.IsNaN(high) || float.IsNaN(seconds) || float.IsInfinity(seconds)) return;
        rumbleRemaining = Mathf.Clamp(seconds, 0f, 2f);
        if (rumbleRemaining <= 0f) { StopRumble(); return; }
        source.SetVibration(Mathf.Clamp01(low), Mathf.Clamp01(high));
    }

    public static void StopRumble()
    {
        rumbleRemaining = 0f;
        source?.SetVibration(0f, 0f);
    }

    internal static void LoseFocus()
    {
        StopRumble();
        Focused = false;
        state.Step(default, false);
        BlockGameplayThisFrame();
    }
}
