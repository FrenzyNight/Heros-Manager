using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMove : MonoBehaviour
{
    private FishManager FM;
    private RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("FishManager").GetComponent<FishManager>();
        rt = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (FM.realFishSpeed * Time.deltaTime), rt.anchoredPosition.y);
        //transform.Translate(-realFishSpeed * Time.deltaTime, 0, 0);
    }

    void GetFish()
    {
       // get RiverCSV.fishNormalRes
       FM.GetFish();
       Debug.Log("Get Fish : " + FM.fishNormalRes.ToString());
       Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Block"))
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
