using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FairyBox : MonoBehaviour
{
    private FairyWarehouseManager FM;
    private RectTransform rt;
    public GameObject GageImg;
    private GameObject Gage;

    private float boxTime = 0;


    private bool isTouch = false;

    public int boxType;
    private float boxGetTime;

    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("FairyWarehouseManager").GetComponent<FairyWarehouseManager>();
        rt = gameObject.GetComponent<RectTransform>();

        Gage = Instantiate(GageImg, rt.position, Quaternion.identity, GameObject.Find("FairyWarehouse").transform);
        Gage.GetComponent<Image>().fillAmount = 0;
        
        if(boxType == 1)
        {
            boxGetTime = FM.fwTreasure1GetTime;
        }
        else if(boxType == 2)
        {
            boxGetTime = FM.fwTreasure2GetTime;
        }
        else if(boxType == 3)
        {
            boxGetTime = FM.fwTreasure3GetTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Gage.GetComponent<Image>().fillAmount = boxTime / boxGetTime;
        
        if(isTouch)
        {
            boxTime += Time.deltaTime;
        }

        if(boxTime >= boxGetTime)
        {
            FM.GetTreasure(boxType);
            Destroy(Gage);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isTouch = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isTouch = false;
        }
    }
}
