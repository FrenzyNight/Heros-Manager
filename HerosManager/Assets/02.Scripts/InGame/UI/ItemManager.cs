using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class ItemManager : Singleton<ItemManager>
{
    public Item[] items;

    static readonly string[] ItemCodes =
        { "Item_Wood", "Item_Water", "Item_Meat", "Item_Hub", "Item_Food" };

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
        var values = new int[ItemCodes.Length];
        for (int i = 0; i < values.Length && i < items.Length; i++)
            if (items[i] != null)
                values[i] = Mathf.Max(0, items[i].num);
        return values;
    }

    public Item GetItemInfo(string _code)
    {
        int index = System.Array.IndexOf(ItemCodes, _code);
        return index >= 0 && items != null && index < items.Length ? items[index] : null;
    }

    public void AddItem(string _code, int _cnt)
    {
        Item item = GetItemInfo(_code);
        if (item == null)
            return;

        item.AddNum(_cnt);
    }

    public void AddItem(int _wood, int _water, int _meat, int _hub, int _food)
    {
        AddItem(ItemCodes[0], _wood);
        AddItem(ItemCodes[1], _water);
        AddItem(ItemCodes[2], _meat);
        AddItem(ItemCodes[3], _hub);
        AddItem(ItemCodes[4], _food);
    }
}
