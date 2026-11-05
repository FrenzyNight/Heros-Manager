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

    private Animator ani;
    private Vector2 initialPosition;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        ani = GetComponent<Animator>();
        initialPosition = rt.anchoredPosition;
    }

    void OnEnable()
    {
        rt.anchoredPosition = initialPosition;
        isStun = isHook = false;
        ani.SetBool("isHook", false);
        ani.SetBool("isStun", false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        isStun = isHook = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        FM = MiniGameReferences.Manager<LakeFishManager>(this);
        rt = gameObject.GetComponent<RectTransform>();
        ani = gameObject.GetComponent<Animator>();
        isStun = false;
        isHook = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(GamepadUiController.MiniGameInputAllowed && !isStun && !isHook)
        {
            if(Input.GetKeyDown(KeyCode.A) || GamepadInput.Pressed(GamepadButton.Left))
            {
                LeftMove();
            }

            if(Input.GetKeyDown(KeyCode.D) || GamepadInput.Pressed(GamepadButton.Right))
            {
                RightMove();
            }

            if(Input.GetKeyDown(KeyCode.Space) || GamepadInput.Pressed(GamepadButton.LeftShoulder))
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
       GamepadInput.Pulse(0.08f, 0.18f, 0.06f);
       ani.SetBool("isHook", true);
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
        ani.SetBool("isStun", true);
        isStun = true;
        GamepadInput.Pulse(0.22f, 0.08f, 0.12f);

        yield return new WaitForSeconds(FM.fishStunTime);

        ani.SetBool("isHook", false);
        ani.SetBool("isStun", false);
        isStun = false;
        isHook = false;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!coll.gameObject.activeSelf) return;
        if(coll.gameObject.CompareTag("Hook"))
        {
            ResetHook();
        }

        if(coll.gameObject.CompareTag("Fish"))
        {
            coll.gameObject.SetActive(false);
            FM.GetFish();
            isHook = false;
            ani.SetBool("isHook", false);
            Destroy(coll.gameObject);
        }
        if(coll.gameObject.CompareTag("GoldFish"))
        {
            coll.gameObject.SetActive(false);
            FM.GetGoldFish();
            isHook = false;
            ani.SetBool("isHook", false);
            Destroy(coll.gameObject);
        }
    }
}
