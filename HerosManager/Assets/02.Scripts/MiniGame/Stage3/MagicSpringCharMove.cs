using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicSpringCharMove : MonoBehaviour
{
    private MagicSpringManager SM;
    private RectTransform rt;

    public float moveVar;
    //private bool isCommand;

    // Start is called before the first frame update
    void Start()
    {
        SM = GameObject.Find("MagicSpringManager").GetComponent<MagicSpringManager>();
        rt = gameObject.GetComponent<RectTransform>();
        //isCommand = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!SM.isCommand)
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

    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.gameObject.CompareTag("Altar"))
        {
            if(!c.gameObject.GetComponent<MagicSpringAltar>().isActive)
            {
                //Debug.Log("command test");
                //SM.isCommand = true;
                Debug.Log("altarNum : " + c.gameObject.GetComponent<MagicSpringAltar>().AltarNum.ToString());
                SM.CommandActive(c.gameObject.GetComponent<MagicSpringAltar>().AltarNum);
            }
        }
    }
}
