using System;
using UnityEditor;
using UnityEngine;

public static class CaveRewardValidation
{
    public static void Run() => Validate();

    [MenuItem("Tools/Heros Manager/Validate Cave Rewards")]
    public static void Validate()
    {
        var ledger = new MiniGameRewardLedger();
        Require(!ledger.AddGems(1), "Closed sessions cannot collect gems.");
        Require(ledger.Begin(), "A fresh session opens.");
        Require(ledger.AddGems(2) && ledger.Add("Item_Wood", 3), "Gems and ordinary rewards share a session.");
        Require(!ledger.Begin() && ledger.Gems == 2, "Duplicate begin preserves collected gems.");
        Require(!ledger.AddGems(-1) && !ledger.AddGems(0) && ledger.Gems == 2,
            "Invalid rewards cannot spend or reset gems.");
        Require(ledger.TryComplete(out var items, out int gems) && gems == 2 && items[0] == 3,
            "One close pays both reward types together.");
        Require(ledger.Gems == 0 && !ledger.TryComplete(out _, out _) && !ledger.AddGems(1),
            "Duplicate close and late pickups cannot create extra currency.");
        ledger.Begin();
        ledger.AddGems(7);
        ledger.Cancel();
        Require(!ledger.TryComplete(out _, out _) && ledger.Gems == 0, "Cancellation discards pending gems.");
        ledger.Begin();
        Require(ledger.Gems == 0 && ledger[0] == 0, "Re-entry begins empty.");
        ledger.AddGems(int.MaxValue);
        ledger.AddGems(1);
        Require(ledger.TryComplete(out _, out gems) && gems == int.MaxValue, "Gem accumulation cannot overflow.");
        Debug.Log("CAVE REWARD VALIDATION PASSED");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
