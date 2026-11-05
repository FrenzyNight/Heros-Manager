using UnityEngine;
using UnityEngine.UI;

/// <summary>Shared presentation for runtime UI; existing illustrated scene assets retain their sprites.</summary>
public static class GameUiStyle
{
    public static readonly Color Surface = new Color(0.075f, 0.13f, 0.14f, 0.97f);
    public static readonly Color Card = new Color(0.12f, 0.20f, 0.20f, 1f);
    public static readonly Color Paper = new Color(0.98f, 0.96f, 0.89f, 1f);
    public static readonly Color Ink = new Color(0.22f, 0.20f, 0.17f, 1f);
    public static readonly Color Accent = new Color(1f, 0.80f, 0.35f, 1f);
    public static readonly Color PositiveInk = new Color(0.12f, 0.36f, 0.25f, 1f);
    public static readonly Color WarningInk = new Color(0.66f, 0.19f, 0.12f, 1f);
    public static readonly Color PositiveText = new Color(0.67f, 0.94f, 0.78f, 1f);
    public static readonly Color WarningText = new Color(1f, 0.75f, 0.57f, 1f);

    public static void TextStyle(Text label, int size, int minimumSize, Color color)
    {
        if (label == null) return;
        label.fontSize = size;
        label.resizeTextForBestFit = minimumSize < size;
        label.resizeTextMinSize = minimumSize;
        label.resizeTextMaxSize = size;
        label.color = color;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Truncate;
        label.lineSpacing = 1.15f;
        label.raycastTarget = false;
    }

    public static void ButtonStyle(Selectable button, bool primary = false)
    {
        if (button == null) return;
        button.transition = Selectable.Transition.ColorTint;
        var colors = button.colors;
        colors.normalColor = primary ? new Color(0.27f, 0.48f, 0.36f) : new Color(0.23f, 0.31f, 0.32f);
        colors.highlightedColor = new Color(0.39f, 0.51f, 0.41f);
        colors.selectedColor = new Color(0.46f, 0.49f, 0.29f);
        colors.pressedColor = new Color(0.16f, 0.28f, 0.24f);
        colors.disabledColor = new Color(0.19f, 0.23f, 0.23f, 0.6f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.12f;
        button.colors = colors;
    }

    public static void Shadow(Graphic graphic)
    {
        var shadow = graphic.GetComponent<Shadow>();
        if (shadow == null) shadow = graphic.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.35f);
        shadow.effectDistance = new Vector2(0, -3);
    }
}
