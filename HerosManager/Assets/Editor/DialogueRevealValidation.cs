using System;

/// <summary>Reveal pace, Unicode boundaries and frame partitioning without a scene or clock.</summary>
public static class DialogueRevealValidation
{
    public static void Run()
    {
        string paragraph = new string('가', 120);
        var slowFrames = new DialogueReveal(paragraph);
        var fastFrames = new DialogueReveal(paragraph);
        for (int i = 0; i < 30; i++) slowFrames.Advance(1d / 30);
        for (int i = 0; i < 120; i++) fastFrames.Advance(1d / 120);
        Require(slowFrames.VisibleLength == 54 && fastFrames.VisibleLength == 54,
            "One second must reveal 54 characters at both 30 and 120 frames per second.");
        var stalledFrame = new DialogueReveal(paragraph);
        stalledFrame.Advance(1);
        Require(stalledFrame.VisibleLength == fastFrames.VisibleLength,
            "A delayed frame must catch up instead of slowing the dialogue permanently.");

        const string punctuation = "천천히, 읽어도 괜찮아요. 다음 문장입니다!";
        var oneStep = new DialogueReveal(punctuation);
        var smallSteps = new DialogueReveal(punctuation);
        oneStep.Advance(0.8);
        for (int i = 0; i < 80; i++) smallSteps.Advance(0.01);
        Require(oneStep.VisibleLength == smallSteps.VisibleLength,
            "Punctuation breaths must be independent of how frames divide the elapsed time.");
        var comma = new DialogueReveal("가,나다");
        comma.Advance(0.08);
        Require(comma.VisibleLength == 2, "A comma must briefly pause before the following character.");
        comma.Advance(0.1);
        Require(comma.IsComplete, "A comma breath must not become a long forced wait.");

        var unicode = new DialogueReveal("A\U0001F600e\u0301한");
        unicode.Advance(1d / 54);
        Require(unicode.VisibleLength == 1, "The first Latin character should reveal normally.");
        unicode.Advance(1d / 54);
        Require(unicode.VisibleLength == 3, "A surrogate pair must reveal together.");
        unicode.Advance(1d / 54);
        Require(unicode.VisibleLength == 5, "A combining accent must remain attached to its base character.");
        unicode.Complete();
        Require(unicode.IsComplete && unicode.VisibleLength == 6 && !unicode.Advance(1),
            "Instant completion must expose the full line and remain complete.");

        var empty = new DialogueReveal(null);
        Require(empty.IsComplete && empty.VisibleLength == 0, "Missing dialogue must finish without a stuck typing state.");
        var invalidFrame = new DialogueReveal("문장");
        invalidFrame.Advance(double.NaN);
        invalidFrame.Advance(-1);
        Require(invalidFrame.VisibleLength == 0, "Invalid clock samples must not corrupt reveal progress.");
    }

    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
