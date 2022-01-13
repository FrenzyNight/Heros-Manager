using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    private HuntManager HM;
    private RectTransform rt;

    private float realArrowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("HuntManager").GetComponent<HuntManager>();
        rt = gameObject.GetComponent<RectTransform>();
        Destroy(gameObject.transform.parent.gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (HM.realArrowSpeed * Time.deltaTime) , rt.anchoredPosition.y);
        //transform.Translate(HM.realArrowSpeed * Time.deltaTime,0,0);
    }
}
