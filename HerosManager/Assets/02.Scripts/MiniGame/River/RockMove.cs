using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMove : MonoBehaviour
{
    private WaterManager WM;
    private RectTransform rt;
    void Start()
    {
        WM = GameObject.Find("WaterManager").GetComponent<WaterManager>();
        rt = gameObject.GetComponent<RectTransform>();
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - (WM.realRockSpeed * Time.deltaTime));
       //transform.Translate(0, -realRockSpeed * Time.deltaTime, 0);
    }
}
