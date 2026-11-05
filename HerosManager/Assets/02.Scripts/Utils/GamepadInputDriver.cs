using UnityEngine;

[DefaultExecutionOrder(-10000)]
public sealed class GamepadInputDriver : MonoBehaviour
{
    private void Update() => GamepadInput.Poll(Time.unscaledDeltaTime, Application.isFocused);
    private void OnApplicationFocus(bool focus) { if (!focus) GamepadInput.LoseFocus(); }
    private void OnApplicationPause(bool pause) { if (pause) GamepadInput.LoseFocus(); }
    private void OnApplicationQuit() => GamepadInput.LoseFocus();
    private void OnDisable() => GamepadInput.LoseFocus();
}
