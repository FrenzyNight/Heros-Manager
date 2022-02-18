using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OasisNetMove : MonoBehaviour
{
    private OasisManager OM;
    private RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        OM = GameObject.Find("OasisManager").GetComponent<OasisManager>();

        rt = gameObject.GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - (OM.realOasisNetSpeed * Time.deltaTime));
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Land"))
        {
            Destroy(gameObject);
        }

        if(coll.gameObject.CompareTag("Water"))
        {
            Destroy(coll.gameObject);
            OM.GetWater();
        }

        if(coll.gameObject.CompareTag("Meat"))
        {
            Destroy(coll.gameObject);
            OM.GetMeat();
        }

        if(coll.gameObject.CompareTag("Tree"))
        {
            Destroy(coll.gameObject);
            OM.GetTree();
        }
    }
}
