using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class ItemManager : Singleton<ItemManager>
{
    public Item[] items;

    protected override void Awake()
    {
        base.Awake();
        if (_instance != this)
            return;
        if (items == null || items.Length == 0 || System.Array.Exists(items, item => item == null))
            items = GetComponentsInChildren<Item>(true);
    }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        //0-wood 1-water 2-meat 3-hub 4-food
        var saved = SaveDataManager.Instance.saveGameData.items;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                items[i].Setup(saved != null && i < saved.Length ? saved[i] : 0);
        }
    }

    public int[] CaptureItems()
    {
        var values = new int[InventoryRules.ItemCount];
        for (int i = 0; items != null && i < values.Length && i < items.Length; i++)
            if (items[i] != null)
                values[i] = Mathf.Max(0, items[i].num);
        return values;
    }

    public Item GetItemInfo(string _code)
    {
        int index = InventoryRules.IndexOf(_code);
        return index >= 0 && items != null && index < items.Length ? items[index] : null;
    }

    public void AddItem(string _code, int _cnt)
    {
        Item item = GetItemInfo(_code);
        if (item == null)
            return;

        item.AddNum(_cnt);
    }

    public int GetAmount(string itemId)
    {
        var item = GetItemInfo(itemId);
        return item != null ? Mathf.Max(0, item.num) : 0;
    }

    public bool CanAfford(string itemId, int amount)
    {
        return CanAfford(new ItemCost(itemId, amount));
    }

    public bool CanAfford(params ItemCost[] costs)
    {
        return HasInventory() && InventoryRules.CanAfford(CaptureItems(), costs);
    }

    public bool TrySpend(string itemId, int amount)
    {
        return TrySpend(new ItemCost(itemId, amount));
    }

    public bool TrySpend(params ItemCost[] costs)
    {
        if (!HasInventory())
            return false;
        var remaining = CaptureItems();
        if (!InventoryRules.TrySpend(remaining, costs))
            return false;
        // Commit every balance before refreshing presentation.
        var deltas = new int[InventoryRules.ItemCount];
        for (int i = 0; i < remaining.Length; i++)
        {
            deltas[i] = remaining[i] - items[i].num;
            items[i].num = remaining[i];
        }
        for (int i = 0; i < remaining.Length; i++)
            items[i].Refresh(deltas[i]);
        return true;
    }

    bool HasInventory()
    {
        if (items == null || items.Length != InventoryRules.ItemCount)
            return false;
        return !System.Array.Exists(items, item => item == null || item.num < 0);
    }

    public void AddItem(int _wood, int _water, int _meat, int _hub, int _food)
    {
        AddItem(InventoryRules.Wood, _wood);
        AddItem(InventoryRules.Water, _water);
        AddItem(InventoryRules.Meat, _meat);
        AddItem(InventoryRules.Herb, _hub);
        AddItem(InventoryRules.Food, _food);
    }
}
