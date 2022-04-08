using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    public int wood = 0;
    public int water = 0;
    public int meat = 0;
    public int hub = 0;
    public int food = 0;

    Item[] items;

    void Start()
    {
        items = this.GetComponentsInChildren<Item>();

        Setup();
    }

    void Setup()
    {
        wood = LoadGameData.Instance.itemDatas["Item_Wood"].FirstGive;
        water = LoadGameData.Instance.itemDatas["Item_Water"].FirstGive;
        meat = LoadGameData.Instance.itemDatas["Item_Meat"].FirstGive;
        hub = LoadGameData.Instance.itemDatas["Item_Hub"].FirstGive;
        food = LoadGameData.Instance.itemDatas["Item_Food"].FirstGive;

        items[0].NumText.text = wood.ToString();
        items[1].NumText.text = water.ToString();
        items[2].NumText.text = meat.ToString();
        items[3].NumText.text = hub.ToString();
        items[4].NumText.text = food.ToString();
    }

    public void AddItem(string _code, int _cnt)
    {
        switch (_code)
        {
            case "Item_Wood":
                wood += _cnt;
                items[0].AddNum(wood, _cnt);
                break;
            case "Item_Water":
                water += _cnt;
                items[1].AddNum(water, _cnt);
                break;
            case "Item_Meat":
                meat += _cnt;
                items[2].AddNum(meat, _cnt);
                break;
            case "Item_Hub":
                hub += _cnt;
                items[3].AddNum(hub, _cnt);
                break;
            case "Item_Food":
                food += _cnt;
                items[4].AddNum(food, _cnt);
                break;
        }
    }
}
