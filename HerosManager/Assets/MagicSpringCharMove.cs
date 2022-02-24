using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicSpringCharMove : MonoBehaviour
{
    private MagicSpringManager SM;
    private RectTransform rt;

    public float moveVar;

    // Start is called before the first frame update
    void Start()
    {
        SM = GameObject.Find("MagicSpringManager").GetComponent<MagicSpringManager>();
        rt = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LeftMove();
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightMove();
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpMove();
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            DownMove();
        }
    }

    void LeftMove()
    {
        if(rt.anchoredPosition.x != -moveVar)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (moveVar), rt.anchoredPosition.y);
        }
    }
    void RightMove()
    {
        if(rt.anchoredPosition.x != moveVar)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (moveVar), rt.anchoredPosition.y);
        }
    }

    void UpMove()
    {
        if(rt.anchoredPosition.y != moveVar)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (moveVar));
        }
    }
    void DownMove()
    {
        if(rt.anchoredPosition.y != -moveVar)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - (moveVar));
        }
    }
}
