using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmuletManager : Singleton<AmuletManager>
{
    public GameObject AmuletNotice;
    public Text NoticeText;

    public GameObject AmuletObj;

    void Start()
    {
        Setup();
    }

    void Setup()
    {

    }

    void SpawnAmulet()
    {
        GameObject obj = Instantiate(AmuletObj, this.transform);
        obj.GetComponent<Amulet>().Setup();
    }
}
