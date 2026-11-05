using System;

/// <summary>Owns uncommitted rewards; a session can be paid out at most once.</summary>
public sealed class MiniGameRewardLedger
{
    private static readonly string[] ItemCodes =
        { "Item_Wood", "Item_Water", "Item_Meat", "Item_Hub", "Item_Food" };
    private readonly int[] amounts = new int[5];

    public bool IsOpen { get; private set; }
    public int Gems { get; private set; }
    public int this[int index] => amounts[index];

    public bool Begin()
    {
        if (IsOpen) return false;
        Array.Clear(amounts, 0, amounts.Length);
        Gems = 0;
        IsOpen = true;
        return true;
    }

    public bool Add(string itemCode, int amount)
    {
        int index = Array.IndexOf(ItemCodes, itemCode);
        if (!IsOpen || index < 0) return false;
        amounts[index] = MiniGameRules.AddReward(amounts[index], amount);
        return true;
    }

    public bool AddGems(int amount)
    {
        if (!IsOpen || amount <= 0) return false;
        Gems = MiniGameRules.AddReward(Gems, amount);
        return true;
    }

    public bool TryComplete(out int[] rewards) => TryComplete(out rewards, out _);

    public bool TryComplete(out int[] rewards, out int gems)
    {
        rewards = null;
        gems = 0;
        if (!IsOpen) return false;
        rewards = (int[])amounts.Clone();
        gems = Gems;
        Cancel();
        return true;
    }

    public void Cancel()
    {
        IsOpen = false;
        Gems = 0;
        Array.Clear(amounts, 0, amounts.Length);
    }
}
