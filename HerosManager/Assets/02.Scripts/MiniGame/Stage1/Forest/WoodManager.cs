using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;

    public GameObject NormalWoodPrefab, GoldWoodPrefab;
    private GameObject NowWood;

    public GameObject WoodChar;
    public float charDistance;
    //UI
    public float widthScale;
    public float heightScale;
    private bool isFirst = true;

    //road
    public float woodStunTime;

    public float woodNormalRes;

    public float woodGoldPer;
    public float woodGoldRes;

    //game
    public int woodDirection;
    public bool isStun;
    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_1_Forest");

        SetUp();
        
        StartCoroutine(FirstStart());
    }

    // Update is called once per frame
    void Update()
    {
        if(!isStun)
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                CheckWoodDirection(0);
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                CheckWoodDirection(1);
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                CheckWoodDirection(2);
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                CheckWoodDirection(3);
            }
        }
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;
        isStun = false;

        woodStunTime = InGameMgr.Instance.miniGameData["game1tree_normal"].stun;

        woodGoldRes = InGameMgr.Instance.miniGameData["game1tree_gold"].wood;
        woodGoldPer = InGameMgr.Instance.miniGameData["game1tree_gold"].probability * 100;

        woodNormalRes = InGameMgr.Instance.miniGameData["game1tree_normal"].wood;
    
        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    void CheckWoodDirection(int input)
    {
        if(input == 0)
        {
            WoodChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, charDistance);
            WoodChar.transform.localScale = new Vector3(1,1,1);
        }
        else if(input == 1)
        {
            WoodChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-charDistance,0);
            WoodChar.transform.localScale = new Vector3(1,1,1);
        }
        else if(input == 2)
        {
            WoodChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -charDistance);
            WoodChar.transform.localScale = new Vector3(-1,-1,1);
        }
        else if(input == 3)
        {
            WoodChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(charDistance,0);
            WoodChar.transform.localScale = new Vector3(-1,1,1);
        }


        if(woodDirection == input)
        {
           if(NowWood.CompareTag("NormalTree"))
           {
               GetNormalWood();
           }
           else if(NowWood.CompareTag("GoldTree"))
           {
               GetGoldWood();
           }

           Destroy(NowWood);
           SpawnWood();
        }
        else
        {
            StartCoroutine(Stun());
        }
    }



    IEnumerator Stun()
    {
        isStun = true;
        Debug.Log("Wood Stun");
        yield return new WaitForSeconds(woodStunTime);
        isStun = false;
    }

    void SpawnWood()
    {
        int rnd = Random.Range(1,101);
        GameObject wood;

        if(rnd <= woodGoldPer)
        {
            wood = GoldWoodPrefab;
        }
        else
        {
            wood = NormalWoodPrefab;
        }

        NowWood = Instantiate(wood, transform.position, Quaternion.identity,GameObject.Find("Wood").transform);

        //set Direction
        woodDirection = Random.Range(0,4);

        if(woodDirection == 0) //상
        {
            NowWood.transform.Rotate(0,0,180);
        }
        else if(woodDirection == 1) //좌
        {
            NowWood.transform.Rotate(0,0,-90);
        }
        else if(woodDirection == 2) //하
        {
            //
        }
        else if(woodDirection == 3)//우
        {
            NowWood.transform.Rotate(0,0,90);
        }
    }

    public void StartGame()
    {
        if(!isFirst)
            StartCoroutine(ReStart());
    }

    void GetNormalWood()
    {
        MGM.wood += (int)woodNormalRes;
    }

    void GetGoldWood()
    {
        MGM.wood += (int)woodGoldRes;
    }

    IEnumerator ReStart()
    {
        GuidPanel.SetActive(true);
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        Destroy(NowWood);
        SpawnWood();
        //StartCoroutine(SpawnObject());
    }
    IEnumerator FirstStart()
    {
        GuidPanel.SetActive(true);
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);
        isFirst = false;

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        SpawnWood();
    }
}
