using System;
using UnityEditor;
using UnityEngine;

public static class GamepadInputValidation
{
    [MenuItem("Tools/Heros Manager/Validate Gamepad Input")]
    public static void Run()
    {
        var state = new GamepadState();
        var neutral = new GamepadFrame { Connected = true };
        var confirm = new GamepadFrame { Connected = true, Buttons = GamepadButton.Confirm };
        state.Step(confirm, true);
        Require(!state.Pressed(GamepadButton.Confirm), "A held while connecting must not activate a menu.");
        state.Step(neutral, true);
        state.Step(confirm, true);
        Require(state.Pressed(GamepadButton.Confirm) && state.Held(GamepadButton.Confirm), "A fresh press is exposed for one frame.");
        state.Step(confirm, true);
        Require(!state.Pressed(GamepadButton.Confirm) && state.Held(GamepadButton.Confirm), "Holding does not repeat submit.");
        state.Step(neutral, true);
        Require(state.Released(GamepadButton.Confirm), "Releases are available for slide/hold actions.");
        state.Step(confirm, false);
        state.Step(confirm, true);
        Require(!state.Held(GamepadButton.Confirm), "Focus regain waits for neutral input.");
        state.Step(neutral, true);
        state.Step(new GamepadFrame { Connected = true, Move = new Vector2(0.1f, 0.1f), Aim = new Vector2(float.NaN, 0) }, true);
        Require(state.Current.Move == Vector2.zero && state.Current.Aim == Vector2.zero, "Stick drift and invalid values are neutral.");
        state.Step(new GamepadFrame { Connected = true, Buttons = GamepadButton.Up | GamepadButton.Right, RightTrigger = 0.8f }, true);
        Require(Mathf.Approximately(state.Current.Move.magnitude, 1f) && state.Pressed(GamepadButton.RightTrigger), "D-pad diagonals are bounded and trigger edges are available.");
        state.Step(default, true);
        Require(!state.Current.Connected && state.Current.Move == Vector2.zero && !state.Held(GamepadButton.RightTrigger), "Disconnect clears movement and held actions.");
        var source = new VirtualSource();
        IGamepadSource previousSource = GamepadInput.SetSourceForTests(source);
        bool previousRumble = GamepadInput.RumbleEnabled;
        try
        {
            source.Frame = neutral;
            GamepadInput.AdvanceFrameForTests(0.016f);
            source.Frame = confirm;
            GamepadInput.AdvanceFrameForTests(0.016f);
            Require(GamepadInput.Pressed(GamepadButton.Confirm), "Virtual source follows the same production polling path.");
            GamepadInput.RumbleEnabled = false;
            Require(source.Stops > 0, "Disabling vibration immediately stops both motors.");
            int before = source.Stops;
            GamepadInput.AdvanceFrameForTests(0.016f, false);
            Require(source.Stops > before && !GamepadInput.Connected, "Focus loss stops motors and clears input.");
            GamepadInput.BlockGameplayThisFrame();
            Require(GamepadInput.GameplayBlockedThisFrame, "UI activation blocks gameplay in the same frame.");
        }
        finally
        {
            GamepadInput.SetSourceForTests(previousSource);
            GamepadInput.RumbleEnabled = previousRumble;
        }
        Debug.Log("GAMEPAD INPUT VALIDATION PASSED (virtual frames; physical controller not required)");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    private sealed class VirtualSource : IGamepadSource
    {
        public GamepadFrame Frame;
        public int Stops;
        public GamepadFrame Read() => Frame;
        public void SetVibration(float low, float high) { if (low == 0 && high == 0) Stops++; }
    }
}
