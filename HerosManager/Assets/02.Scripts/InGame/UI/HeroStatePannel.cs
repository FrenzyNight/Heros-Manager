using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStatePannel : MonoBehaviour
{
    HeroState[] heroStateList;

    void Start()
    {
        heroStateList = this.GetComponentsInChildren<HeroState>();
    }

    //void Update()
    //{
        
    //}
}
