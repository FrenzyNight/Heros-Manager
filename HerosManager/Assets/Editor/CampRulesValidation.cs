using System;
using UnityEngine;

/// <summary>Economic regression checks. Never reads or writes player saves.</summary>
public static class CampRulesValidation
{
    public static void Run()
    {
        var stock = new[] { 10, 5, 3, 2, 0 };
        var before = (int[])stock.Clone();
        Require(!InventoryRules.TrySpend(stock, new[]
        {
            new ItemCost(InventoryRules.Wood, 7), new ItemCost(InventoryRules.Water, 6)
        }), "Insufficient second resource must reject the entire recipe.");
        Same(stock, before, "Rejected multi-resource spend changed stock.");

        Require(!InventoryRules.TrySpend(stock, new[]
        {
            new ItemCost(InventoryRules.Wood, 6), new ItemCost(InventoryRules.Wood, 5)
        }), "Repeated recipe ingredients must be aggregated.");
        Same(stock, before, "Rejected repeated ingredient changed stock.");

        Require(InventoryRules.TrySpend(stock, new[]
        {
            new ItemCost(InventoryRules.Wood, 4), new ItemCost(InventoryRules.Wood, 6),
            new ItemCost(InventoryRules.Water, 5), new ItemCost("-1", 0)
        }), "Exact funds and an absent optional ingredient must succeed.");
        Same(stock, new[] { 0, 0, 3, 2, 0 }, "A successful recipe must debit each resource exactly once.");
        Require(!InventoryRules.TrySpend(stock, new[] { new ItemCost(InventoryRules.Wood, 1) }),
            "A repeated purchase must fail after funds are spent.");

        before = (int[])stock.Clone();
        Require(!InventoryRules.TrySpend(stock, new[] { new ItemCost(InventoryRules.Meat, -1) }),
            "Negative costs must not mint resources.");
        Require(!InventoryRules.TrySpend(stock, new[] { new ItemCost("unknown", 0) }),
            "Unknown item IDs must not silently pass.");
        Require(!InventoryRules.TrySpend(stock, null), "Null recipes must be rejected.");
        Same(stock, before, "Malformed recipes changed stock.");
        Require(InventoryRules.CanAfford(stock, Array.Empty<ItemCost>()), "An explicit free action is valid.");
        Require(!InventoryRules.CanAfford(new int[4], Array.Empty<ItemCost>()), "Malformed inventory must be rejected.");

        var maximum = new[] { int.MaxValue, 0, 0, 0, 0 };
        Require(!InventoryRules.TrySpend(maximum, new[]
        {
            new ItemCost(InventoryRules.Wood, int.MaxValue), new ItemCost(InventoryRules.Wood, 1)
        }), "Summed costs must not overflow.");
        Require(maximum[0] == int.MaxValue, "Overflow rejection changed stock.");
        Require(InventoryRules.ApplyDelta(int.MaxValue, 1) == int.MaxValue, "Reward overflow must saturate.");
        Require(InventoryRules.ApplyDelta(3, -8) == 0, "Invasion losses must stop at zero.");
        Require(InventoryRules.ApplyDelta(0, int.MinValue) == 0, "Extreme losses must not overflow.");

        ValidateManagerBoundary();
        Debug.Log("CAMP RULES VALIDATION PASSED");
    }

    static void ValidateManagerBoundary()
    {
        var root = new GameObject("Inventory validation");
        try
        {
            var manager = root.AddComponent<ItemManager>();
            manager.items = new Item[InventoryRules.ItemCount];
            for (int i = 0; i < manager.items.Length; i++)
            {
                var child = new GameObject("Item " + i);
                child.transform.SetParent(root.transform);
                manager.items[i] = child.AddComponent<Item>();
                manager.items[i].Setup(10);
            }
            Require(manager.CanAfford(InventoryRules.Water, 10), "Manager affordability disagrees with stock.");
            Require(manager.TrySpend(new ItemCost(InventoryRules.Water, 4), new ItemCost(InventoryRules.Meat, 10)),
                "Manager rejected a funded transaction.");
            Same(manager.CaptureItems(), new[] { 10, 6, 0, 10, 10 }, "Manager committed incorrect balances.");
            Require(!manager.TrySpend(new ItemCost(InventoryRules.Water, 6), new ItemCost(InventoryRules.Meat, 1)),
                "Manager accepted an unfunded transaction.");
            Same(manager.CaptureItems(), new[] { 10, 6, 0, 10, 10 }, "Manager partially applied a rejected transaction.");
        }
        finally { UnityEngine.Object.DestroyImmediate(root); }
    }

    static void Same(int[] actual, int[] expected, string message)
    {
        Require(actual.Length == expected.Length, message);
        for (int i = 0; i < actual.Length; i++)
            Require(actual[i] == expected[i], message);
    }

    static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
