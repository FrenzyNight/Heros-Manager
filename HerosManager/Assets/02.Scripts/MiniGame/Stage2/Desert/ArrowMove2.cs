using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove2 : MonoBehaviour
{
    private Hunt2Manager HM;
    private RectTransform rt;

    private float realArrowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("Hunt2Manager").GetComponent<Hunt2Manager>();
        rt = gameObject.GetComponent<RectTransform>();
        Destroy(gameObject.transform.parent.gameObject, 1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (HM.realArrowSpeed * Time.deltaTime) , rt.anchoredPosition.y);
        //transform.Translate(HM.realArrowSpeed * Time.deltaTime,0,0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Cactus"))
        {
            Destroy(gameObject);
        }
    }
}
