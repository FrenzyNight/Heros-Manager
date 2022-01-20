using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        wood = LoadGameData.Instance.itemDatas["wood"].startNum;
        water = LoadGameData.Instance.itemDatas["water"].startNum;
        meat = LoadGameData.Instance.itemDatas["meat"].startNum;
        hub = LoadGameData.Instance.itemDatas["hub"].startNum;
        food = LoadGameData.Instance.itemDatas["food"].startNum;

        items[0].NumText.text = wood.ToString();
        items[1].NumText.text = water.ToString();
        items[2].NumText.text = meat.ToString();
        items[3].NumText.text = hub.ToString();
        items[4].NumText.text = food.ToString();
    }

    public void AddItem(int _wood, int _water, int _meat, int _hub, int _food)
    {
        wood += _wood;
        water += _water;
        meat += _meat;
        hub += _hub;
        food += _food;

        items[0].AddNum(wood, _wood);
        items[1].AddNum(water, _water);
        items[2].AddNum(meat, _meat);
        items[3].AddNum(hub, _hub);
        items[4].AddNum(food, _food);
    }
}
