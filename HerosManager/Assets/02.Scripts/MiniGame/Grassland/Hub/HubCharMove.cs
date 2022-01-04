using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubCharMove : MonoBehaviour
{
    private HubManager HM;
    
    private RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("HubManager").GetComponent<HubManager>();
        rt = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (HM.realHubCharSpeed * Time.deltaTime));
            //tr.Translate(Vector2.up * HM.realHubCharSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A))
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (HM.realHubCharSpeed * Time.deltaTime), rt.anchoredPosition.y);
            //tr.Translate(Vector2.left * HM.realHubCharSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S))
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - (HM.realHubCharSpeed * Time.deltaTime));
            //tr.Translate(Vector2.down * HM.realHubCharSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D))
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (HM.realHubCharSpeed * Time.deltaTime), rt.anchoredPosition.y);
            //tr.Translate(Vector2.right * HM.realHubCharSpeed * Time.deltaTime);
        }

    }
}
