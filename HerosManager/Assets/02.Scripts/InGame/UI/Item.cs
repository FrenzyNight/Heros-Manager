using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : Singleton<Item>
{
    public int wood = 0;
    public int water = 0;
    public int meat = 0;
    public int hub = 0;
    public int jem = 0;

    Transform[] items = new Transform[5];

    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
            items[i] = this.transform.GetChild(i);
    }

    public void AddItem(string _code, int _num)
    {
        switch (_code)
        {
            case "Wood":
                wood += _num;
                items[0].GetComponentInChildren<Text>().text = "X" + wood;
                break;
            case "Water":
                water += _num;
                items[1].GetComponentInChildren<Text>().text = "X" + water;
                break;
            case "Meat":
                meat += _num;
                items[2].GetComponentInChildren<Text>().text = "X" + meat;
                break;
            case "Hub":
                hub += _num;
                items[3].GetComponentInChildren<Text>().text = "X" + hub;
                break;
            case "Jem":
                jem += _num;
                items[4].GetComponentInChildren<Text>().text = "X" + jem;
                break;
        }
    }
}
