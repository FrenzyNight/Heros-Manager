using System;
using System.Globalization;

/// <summary>Frame-rate independent reveal timing. Boundaries preserve complete Unicode text elements.</summary>
public sealed class DialogueReveal
{
    public const double CharactersPerSecond = 54;
    readonly string text;
    readonly int[] starts;
    readonly double characterInterval;
    double elapsed, nextReveal;
    int visibleElements;

    public DialogueReveal(string value, double charactersPerSecond = CharactersPerSecond)
    {
        text = value ?? string.Empty;
        starts = StringInfo.ParseCombiningCharacters(text);
        if (double.IsNaN(charactersPerSecond) || double.IsInfinity(charactersPerSecond) || charactersPerSecond <= 0)
            throw new ArgumentOutOfRangeException(nameof(charactersPerSecond));
        characterInterval = 1 / charactersPerSecond;
        nextReveal = characterInterval;
    }

    public bool IsComplete => visibleElements == starts.Length;
    public int VisibleLength => IsComplete ? text.Length : starts[visibleElements];

    public bool Advance(double unscaledSeconds)
    {
        if (IsComplete || double.IsNaN(unscaledSeconds) || double.IsInfinity(unscaledSeconds) || unscaledSeconds <= 0)
            return false;
        elapsed += unscaledSeconds;
        int before = visibleElements;
        while (!IsComplete && elapsed + 0.0000001 >= nextReveal)
        {
            int revealed = visibleElements++;
            nextReveal += characterInterval + PauseAfter(revealed);
        }
        return before != visibleElements;
    }

    public void Complete() => visibleElements = starts.Length;

    double PauseAfter(int index)
    {
        char character = text[starts[index]];
        // An ellipsis or repeated punctuation takes one breath, not one pause per mark.
        if (index + 1 < starts.Length && IsPunctuation(text[starts[index + 1]])) return 0;
        if (character == '.' || character == '!' || character == '?' || character == '…' || character == '。') return 0.14;
        if (character == ',' || character == ';' || character == ':' || character == '，') return 0.065;
        return character == '\n' ? 0.08 : 0;
    }

    static bool IsPunctuation(char character) => ".!?…,;:。，".IndexOf(character) >= 0;
}
