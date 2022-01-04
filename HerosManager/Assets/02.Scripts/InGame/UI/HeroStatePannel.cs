using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStatePannel : MonoBehaviour
{
    HeroState[] heroStateList;

    void Start()
    {
        heroStateList = this.GetComponentsInChildren<HeroState>();
        for (int i = 0; i < heroStateList.Length; i++)
            heroStateList[i].SetState(InGameMgr.Instance.heroInfos[i]);
    }

    //void Update()
    //{
        
    //}
}
