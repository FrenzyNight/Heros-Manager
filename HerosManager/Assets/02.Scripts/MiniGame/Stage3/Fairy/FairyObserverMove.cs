using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyObserverMove : MonoBehaviour
{
    public GameObject Back, Front, Left, Right;
    private FairyWarehouseManager FM;
    private RectTransform rt;
    private bool isObserve;
    public float fairySpeed;
    public int direction;
    public float topP, botP;
    public float observePoint1, observePoint2;
    private bool obCheck1, obCheck2;

    public int fairyNum;

    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("FairyWarehouseManager").GetComponent<FairyWarehouseManager>();
        rt = gameObject.GetComponent<RectTransform>();

        isObserve = false;
        obCheck1 = false;
        obCheck2 = false;
        
        SetUp();
    }

    void SetUp()
    {
        if(fairyNum == 1)
        {
            fairySpeed = FM.fwFairy1RealSpeed;
        }
        else if(fairyNum == 2)
        {
            fairySpeed = FM.fwFairy2RealSpeed;
        }
        else if(fairyNum == 3)
        {
            fairySpeed = FM.fwFairy3RealSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(direction == 1)
        {
            if(rt.anchoredPosition.y >= observePoint1 && !obCheck1)
            {
                obCheck1 = true;
                StartCoroutine(Observe());
            }
            if(rt.anchoredPosition.y >= observePoint2 && !obCheck2)
            {
                obCheck2 = true;
                StartCoroutine(Observe());
            }

            if(rt.anchoredPosition.y >= topP)
            {
                SetDirection(-1);
            }
        }

        else if(direction == -1)
        {
            if(rt.anchoredPosition.y <= observePoint1 && !obCheck1)
            {
                obCheck1 = true;
                StartCoroutine(Observe());
            }
            if(rt.anchoredPosition.y <= observePoint2 && !obCheck2)
            {
                obCheck2 = true;
                StartCoroutine(Observe());
            }
            if(rt.anchoredPosition.y <= botP)
            {
                SetDirection(1);
            }
        }
        
        
    }

    void FixedUpdate()
    {
        if(!isObserve)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (fairySpeed * direction * Time.deltaTime));
        }
    }

    void SetDirection(int dir)
    {
        direction = dir;
        obCheck1 = false;
        obCheck2 = false;

        if(direction == 1)
        {
            Front.SetActive(false);
            Back.SetActive(true);
        }
        else if(direction == -1)
        {
            Back.SetActive(false);
            Front.SetActive(true);
        }
    }

    IEnumerator Observe()
    {
        isObserve = true;

        if(direction == 1)
        {
            Back.SetActive(false);
            Left.SetActive(true);

            yield return new WaitForSeconds(FM.fwFairyLeftTime);

            Left.SetActive(false);
            Back.SetActive(true);

            yield return new WaitForSeconds(FM.fwFairyFrontTime);

            Back.SetActive(false);
            Right.SetActive(true);

            yield return new WaitForSeconds(FM.fwFairyRightTime);

            Right.SetActive(false);
            Back.SetActive(true);
        }

        else if(direction == -1)
        {
            Front.SetActive(false);
            Right.SetActive(true);

            yield return new WaitForSeconds(FM.fwFairyRightTime);

            Right.SetActive(false);
            Front.SetActive(true);

            yield return new WaitForSeconds(FM.fwFairyFrontTime);

            Front.SetActive(false);
            Left.SetActive(true);

            yield return new WaitForSeconds(FM.fwFairyLeftTime);

            Left.SetActive(false);
            Front.SetActive(true);
        }

        isObserve = false;
    }
}
