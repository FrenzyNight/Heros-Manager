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
    public int jem = 0;

    Item[] items;

    void Start()
    {
        items = this.GetComponentsInChildren<Item>();
    }

    private void Update()
    {
        
    }

    public void AddItem(int _wood, int _water, int _meat, int _hub, int _jem)
    {
        wood += _wood;
        water += _water;
        meat += _meat;
        hub += _hub;
        jem += _jem;

        items[0].AddNum(wood, _wood);
        items[1].AddNum(water, _water);
        items[2].AddNum(meat, _meat);
        items[3].AddNum(hub, _hub);
        items[4].AddNum(jem, _jem);
    }
}
