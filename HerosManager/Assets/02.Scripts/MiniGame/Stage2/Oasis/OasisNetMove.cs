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
        OM = MiniGameReferences.Manager<OasisManager>(this);

        rt = gameObject.GetComponent<RectTransform>();
        Destroy(transform.parent.gameObject, 6f);
    }

    void FixedUpdate()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - (OM.realOasisNetSpeed * Time.deltaTime));
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!gameObject.activeSelf || !coll.gameObject.activeSelf) return;
        if(coll.gameObject.CompareTag("Land"))
        {
            gameObject.SetActive(false);
            Destroy(transform.parent.gameObject);
            return;
        }

        if(coll.gameObject.CompareTag("Water"))
        {
            coll.gameObject.SetActive(false);
            Destroy(coll.transform.parent.gameObject);
            OM.GetWater();
        }

        if(coll.gameObject.CompareTag("Meat"))
        {
            coll.gameObject.SetActive(false);
            Destroy(coll.transform.parent.gameObject);
            OM.GetMeat();
        }

        if(coll.gameObject.CompareTag("Tree"))
        {
            coll.gameObject.SetActive(false);
            Destroy(coll.transform.parent.gameObject);
            OM.GetTree();
        }
    }
}
