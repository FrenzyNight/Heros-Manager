using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeHookMove : MonoBehaviour
{
    private LakeFishManager FM;
    private RectTransform rt;

    public bool isHookDown, isHookUp;
    public bool isMissing;
    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("LakeFishManager").GetComponent<LakeFishManager>();
        rt = gameObject.GetComponent<RectTransform>();

        isHookDown = false;
        isHookUp = false;
        isMissing = false;
    }

    public void HookDown()
    {
        isHookDown = true;
        isMissing = false;
    }

    void FixedUpdate()
    {
        if(isHookDown)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - (FM.realFishLineSpeed * Time.deltaTime));
        }

        if(isHookUp)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (FM.realFishLineSpeed * FM.fishLineCatchSpeedVar * Time.deltaTime));
        }
    
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Bottom"))
        {
            isHookDown = false;
            isHookUp = true;
            isMissing = true;
        }

        if(coll.gameObject.CompareTag("Fish") || coll.gameObject.CompareTag("GoldFish"))
        {
            isHookDown = false;
            isHookUp = true;
            isMissing = false;
        }

        if(coll.gameObject.CompareTag("Player"))
        {
            isHookDown = false;
            isHookUp = false;
        }
    }
}
