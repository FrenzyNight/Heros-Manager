using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>Windows XInput only. Unsupported platforms/drivers safely report disconnected.</summary>
public sealed class WindowsGamepadSource : IGamepadSource
{
    private int player = -1;
    private bool unavailable;
    [StructLayout(LayoutKind.Sequential)]
    private struct NativeGamepad
    {
        public ushort buttons;
        public byte leftTrigger, rightTrigger;
        public short leftX, leftY, rightX, rightY;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct NativeState { public uint packet; public NativeGamepad gamepad; }
    [StructLayout(LayoutKind.Sequential)]
    private struct Vibration { public ushort low, high; }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
    private static extern uint GetState(uint player, out NativeState state);
    [DllImport("xinput1_4.dll", EntryPoint = "XInputSetState")]
    private static extern uint SetState(uint player, ref Vibration vibration);
#endif
    public GamepadFrame Read()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (unavailable) return default;
        try
        {
            NativeState sample;
            if (player >= 0 && GetState((uint)player, out sample) == 0) return Convert(sample);
            if (player >= 0)
            {
                SetVibration(0, 0);
                player = -1;
                return default;
            }
            for (int index = 0; index < 4; index++)
                if (GetState((uint)index, out sample) == 0)
                {
                    player = index;
                    SetVibration(0, 0);
                    return Convert(sample);
                }
        }
        catch (DllNotFoundException) { unavailable = true; }
        catch (EntryPointNotFoundException) { unavailable = true; }
        catch (BadImageFormatException) { unavailable = true; }
#endif
        return default;
    }
    private static GamepadFrame Convert(NativeState state)
    {
        var pad = state.gamepad;
        return new GamepadFrame
        {
            Connected = true, Buttons = (GamepadButton)pad.buttons,
            Move = new Vector2(Mathf.Max(-1f, pad.leftX / 32767f), Mathf.Max(-1f, pad.leftY / 32767f)),
            Aim = new Vector2(Mathf.Max(-1f, pad.rightX / 32767f), Mathf.Max(-1f, pad.rightY / 32767f)),
            LeftTrigger = pad.leftTrigger / 255f, RightTrigger = pad.rightTrigger / 255f
        };
    }
    public void SetVibration(float low, float high)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (unavailable || player < 0) return;
        try
        {
            var vibration = new Vibration { low = (ushort)(Mathf.Clamp01(low) * ushort.MaxValue), high = (ushort)(Mathf.Clamp01(high) * ushort.MaxValue) };
            SetState((uint)player, ref vibration);
        }
        catch (DllNotFoundException) { unavailable = true; }
        catch (EntryPointNotFoundException) { unavailable = true; }
        catch (BadImageFormatException) { unavailable = true; }
#endif
    }
}
