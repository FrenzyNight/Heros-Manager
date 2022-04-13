using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    Item[] items;

    void Start()
    {
        items = this.GetComponentsInChildren<Item>();

        Setup();
    }

    void Setup()
    {
        //0-wood 1-water 2-meat 3-hub 4-food
        items[0].Setup(LoadGameData.Instance.itemDatas["Item_Wood"]);
        items[1].Setup(LoadGameData.Instance.itemDatas["Item_Water"]);
        items[2].Setup(LoadGameData.Instance.itemDatas["Item_Meat"]);
        items[3].Setup(LoadGameData.Instance.itemDatas["Item_Hub"]);
        items[4].Setup(LoadGameData.Instance.itemDatas["Item_Food"]);
    }

    Item GetItemInfo(string _code)
    {
        Item item = null;

        switch (_code)
        {
            case "Item_Wood":
                item = items[0];
                break;
            case "Item_Water":
                item = items[1];
                break;
            case "Item_Meat":
                item = items[2];
                break;
            case "Item_Hub":
                item = items[3];
                break;
            case "Item_Food":
                item = items[4];
                break;
        }

        return item;
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
        items[0].AddNum(_wood);
        items[1].AddNum(_water);
        items[2].AddNum(_meat);
        items[3].AddNum(_hub);
        items[4].AddNum(_food);
    }
}
