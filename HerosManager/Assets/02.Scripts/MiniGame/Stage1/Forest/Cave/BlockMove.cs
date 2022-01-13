using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : MonoBehaviour
{
    private CaveManager CM;
    private RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        CM = GameObject.Find("CaveManager").GetComponent<CaveManager>();
        rt = gameObject.GetComponent<RectTransform>();
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(CM.isRun)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (CM.realSpeed * Time.deltaTime) , rt.anchoredPosition.y);
            //transform.Translate( - CM.realSpeed * Time.deltaTime, 0, 0);
        }
    
    }
}
