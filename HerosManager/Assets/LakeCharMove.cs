using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeCharMove : MonoBehaviour
{
    private LakeFishManager FM;
    private RectTransform rt;

    public GameObject hook;
    public float moveVar;

    private bool isStun;
    private bool isHook;


    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("LakeFishManager").GetComponent<LakeFishManager>();
        rt = gameObject.GetComponent<RectTransform>();

        isStun = false;
        isHook = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isStun && !isHook)
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                LeftMove();
            }

            if(Input.GetKeyDown(KeyCode.D))
            {
                RightMove();
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                HookDown();
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

    void HookDown()
    {
       isHook = true;
       hook.GetComponent<LakeHookMove>().HookDown();
    }

    void ResetHook()
    {
        
        if(hook.GetComponent<LakeHookMove>().isMissing)
        {
            StartCoroutine(Stun());
        }
        
    }

    IEnumerator Stun()
    {
        isStun = true;
        Debug.Log("Stun");

        yield return new WaitForSeconds(FM.fishStunTime);

        isStun = false;
        isHook = false;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Hook"))
        {
            ResetHook();
        }

        if(coll.gameObject.CompareTag("Fish"))
        {
            FM.GetFish();
            isHook = false;
            Destroy(coll.gameObject);
        }
        if(coll.gameObject.CompareTag("GoldFish"))
        {
            FM.GetGoldFish();
            isHook = false;
            Destroy(coll.gameObject);
        }
    }
}
