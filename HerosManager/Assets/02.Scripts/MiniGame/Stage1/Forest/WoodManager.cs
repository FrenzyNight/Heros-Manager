using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManager : MiniGameSetMgr
{

    public GameObject NormalWoodPrefab, GoldWoodPrefab;
    private GameObject NowWood;

    public GameObject WoodChar;
    

    //road
    public float woodStunTime;

    public int woodNormalRes;

    public float woodGoldPer;
    public int woodGoldRes;

    //game
    public int woodDirection;
    public bool isStun;
    public float charDistance;
    
    void Start()
    {
        SetUp();
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

    public override void SetUp()
    {
        base.SetUp();
        isStun = false;

        woodStunTime = LoadGameData.Instance.miniGameDatas["game1tree_normal"].Stun;

        woodGoldRes = (int)LoadGameData.Instance.miniGameDatas["game1tree_gold"].GetAmount1;
        woodGoldPer = LoadGameData.Instance.miniGameDatas["game1tree_gold"].Probability * 100;

        woodNormalRes = (int)LoadGameData.Instance.miniGameDatas["game1tree_normal"].GetAmount1;

        item1ID = LoadGameData.Instance.miniGameDatas["game1tree_normal"].GetItemID1;
    
        StartGame();
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
               AddItem(item1ID,woodNormalRes);
           }
           else if(NowWood.CompareTag("GoldTree"))
           {
               AddItem(item1ID,woodGoldRes);
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

    public override void StartGame()
    {
        base.StartGame();

        SpawnWood();
    }

}
