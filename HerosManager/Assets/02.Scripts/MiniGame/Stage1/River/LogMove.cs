using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMove : MonoBehaviour
{
    private FishManager FM;
    private RectTransform rt;
    public float direction;

    //public Vector2 limitUp, limitDown;
    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("FishManager").GetComponent<FishManager>();
        rt = gameObject.GetComponent<RectTransform>();
        direction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (FM.realLogSpeed * direction * Time.deltaTime));
        //transform.Translate(0,realLogSpeed * direction * Time.deltaTime,0);

        //Debug.Log("position y = " + transform.position.y.ToString());
        
        
        if(rt.anchoredPosition.y >= FM.topP)
        {
            direction = -1;
        }
        else if(rt.anchoredPosition.y <= FM.botP)
        {
            direction = 1;
        }
        
    }
}
