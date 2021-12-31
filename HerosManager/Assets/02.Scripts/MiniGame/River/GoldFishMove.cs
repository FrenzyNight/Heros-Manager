using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldFishMove : MonoBehaviour
{
    private RiverCSVReader RiverCSV;
    private float realFishSpeed;

    // Start is called before the first frame update
    void Start()
    {
        RiverCSV = GameObject.Find("RiverCSVReader").GetComponent<RiverCSVReader>();
        realFishSpeed = RiverCSV.fishVarSpeed * RiverCSV.standardFishSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-realFishSpeed * Time.deltaTime, 0, 0);
    }

    void GetFish()
    {
       // get RiverCSV.fishGoldRes
       Debug.Log("Get GoldFish : " + RiverCSV.fishGoldRes.ToString());
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
