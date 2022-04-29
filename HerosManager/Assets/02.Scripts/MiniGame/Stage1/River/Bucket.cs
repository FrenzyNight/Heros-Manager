﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bucket : MiniGameObjectMgr
{
    private WaterManager WM;
    private Image bucketImg;

    public Sprite img1, img2, img3, img4;

    public float waterTime;
    // Start is called before the first frame update
    void Start()
    {
        WM = manager.GetComponent<WaterManager>();
        bucketImg = gameObject.GetComponent<Image>();
        waterTime = 0;

        objectType = 2;
    }

    public override void UpdateAction()
    {
        waterTime += Time.deltaTime;

        if(waterTime < WM.waterGetTime / 4f)
        {
            bucketImg.sprite = img1;
        }
        else if(waterTime < WM.waterGetTime / 4f * 2f)
        {
            bucketImg.sprite = img2;
        }
        else if(waterTime < WM.waterGetTime / 4f * 3f)
        {
            bucketImg.sprite = img3;
        }
        else if(waterTime < WM.waterGetTime / 4f * 4f)
        {
            bucketImg.sprite = img4;
        }
        else if(waterTime >= WM.waterGetTime)
        {
            GetWater();
        }
    }


    void GetWater()
    {
        Debug.Log("Get Water : " + WM.waterRes.ToString());
        WM.GetWater();
        waterTime = 0;
    }
}
