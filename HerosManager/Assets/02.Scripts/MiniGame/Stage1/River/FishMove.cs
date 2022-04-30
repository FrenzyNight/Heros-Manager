using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMove : MiniGameObjectMgr
{
    private FishManager FM;
    private RectTransform rt;
    public bool isGold;

    // Start is called before the first frame update
    void Start()
    {
        FM = manager.GetComponent<FishManager>();
        rt = gameObject.GetComponent<RectTransform>();

        objectType = 1;
    }

    public override void MoveAction()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (FM.realFishSpeed * Time.deltaTime), rt.anchoredPosition.y);
    }

    void GetFish()
    {
        if(isGold)
        {
            FM.GetGoldFish();
            Debug.Log("Get GoldFish : " + FM.fishGoldRes.ToString());
        }
        else
        {
            FM.GetFish();
            Debug.Log("Get Fish : " + FM.fishNormalRes.ToString());
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("MiniGameObj1")) //block
        {
            //Debug.Log("block");
            Destroy(gameObject);
        }

        if(coll.gameObject.CompareTag("Player"))
        {
            GetFish();
        }
    }
}
