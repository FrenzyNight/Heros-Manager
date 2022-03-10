using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyObserverMove : MonoBehaviour
{
    private FairyWarehouseManager FM;
    private RectTransform rt;
    private bool isObserve;
    private float fairySpeed;
    public int direction;
    public float topP, botP;

    public int fairyNum;

    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("FairyWarehouseManager").GetComponent<FairyWarehouseManager>();
        rt = gameObject.GetComponent<RectTransform>();

        isObserve = false;
        
        SetUp();
    }

    void SetUp()
    {
        if(fairyNum == 1)
        {
            fairySpeed = FM.fwFairy1RealSpeed;
        }
        else if(fairyNum == 2)
        {
            fairySpeed = FM.fwFairy2RealSpeed;
        }
        else if(fairyNum == 3)
        {
            fairySpeed = FM.fwFairy3RealSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(rt.anchoredPosition.y >= topP)
        {
            direction = -1;
        }
        else if(rt.anchoredPosition.y <= botP)
        {
            direction = 1;
        }
    }

    void FixedUpdate()
    {
        if(!isObserve)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (fairySpeed * direction * Time.deltaTime));
        }
    }

}
