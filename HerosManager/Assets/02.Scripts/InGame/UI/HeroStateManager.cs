using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStateManager : MonoBehaviour
{
    public HeroInfo[] heroInfos = new HeroInfo[4];
    HeroState[] heroStateList;

    void Start()
    {
        heroStateList = this.GetComponentsInChildren<HeroState>();

        Setup();
    }

    void Setup()
    {
        for (int i = 0; i < heroInfos.Length; i++)
        {
            HeroInfo hInfo = new HeroInfo(100f, 100f, 0f, 0f);
            heroInfos[i] = hInfo;
            heroStateList[i].SetState(hInfo);
        }
    }

    //void Update()
    //{

    //}
}

public class HeroInfo
{
    public float hp;
    public float power;
    public float stress;
    public float bravery;

    public HeroInfo(float _hp, float _power, float _stress, float _bravery)
    {
        this.hp = _hp;
        this.power = _power;
        this.stress = _stress;
        this.bravery = _bravery;
    }
}