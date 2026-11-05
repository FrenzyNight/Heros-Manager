using System;
using System.Collections.Generic;

public readonly struct ItemCost
{
    public readonly string ItemId;
    public readonly int Amount;

    public ItemCost(string itemId, int amount)
    {
        ItemId = itemId;
        Amount = amount;
    }
}

/// <summary>Item IDs and save-array order shared by inventory transactions.</summary>
public static class InventoryRules
{
    public const string Wood = "Item_Wood";
    public const string Water = "Item_Water";
    public const string Meat = "Item_Meat";
    public const string Herb = "Item_Hub";
    public const string Food = "Item_Food";
    public const int ItemCount = 5;

    public static int IndexOf(string id)
    {
        switch (id)
        {
            case Wood: return 0;
            case Water: return 1;
            case Meat: return 2;
            case Herb: return 3;
            case Food: return 4;
            default: return -1;
        }
    }

    public static bool CanAfford(IReadOnlyList<int> inventory, IReadOnlyList<ItemCost> costs)
    {
        return TryGetTotals(inventory, costs, out _);
    }

    /// <summary>All costs commit together; a rejected transaction leaves inventory untouched.</summary>
    public static bool TrySpend(int[] inventory, IReadOnlyList<ItemCost> costs)
    {
        if (!TryGetTotals(inventory, costs, out var totals))
            return false;
        for (int i = 0; i < ItemCount; i++)
            inventory[i] -= totals[i];
        return true;
    }

    static bool TryGetTotals(IReadOnlyList<int> inventory, IReadOnlyList<ItemCost> costs, out int[] totals)
    {
        totals = null;
        if (inventory == null || inventory.Count != ItemCount || costs == null)
            return false;
        for (int i = 0; i < ItemCount; i++)
            if (inventory[i] < 0)
                return false;

        totals = new int[ItemCount];
        for (int i = 0; i < costs.Count; i++)
        {
            var cost = costs[i];
            if (cost.Amount < 0)
                return false;
            // Optional recipe columns use -1 with zero amount in the original data.
            if (cost.Amount == 0 && (string.IsNullOrEmpty(cost.ItemId) || cost.ItemId == "-1"))
                continue;
            int index = IndexOf(cost.ItemId);
            if (index < 0 || (long)totals[index] + cost.Amount > inventory[index])
                return false;
            totals[index] += cost.Amount;
        }
        return true;
    }

    public static int ApplyDelta(int current, int delta)
    {
        return (int)Math.Max(0L, Math.Min(int.MaxValue, (long)current + delta));
    }
}
