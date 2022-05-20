using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManager : MiniGameSetMgr
{
    AudioSource audioSource;
    public AudioClip woodSuccessSound, woodFailSound;

    public GameObject NormalWoodPrefab, GoldWoodPrefab;
    private GameObject NowWood;

    public GameObject WoodChar;

    public GameObject IdleObj, StunObj, AxeObj;
    

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
        //SetUp();
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
        Destroy(NowWood);

        audioSource = gameObject.GetComponent<AudioSource>();

        woodStunTime = LoadGameData.Instance.miniGameDatas["game1tree_normal"].Stun;

        woodGoldRes = (int)LoadGameData.Instance.miniGameDatas["game1tree_gold"].GetAmount1;
        woodGoldPer = LoadGameData.Instance.miniGameDatas["game1tree_gold"].Probability * 100;

        woodNormalRes = (int)LoadGameData.Instance.miniGameDatas["game1tree_normal"].GetAmount1;

        item1ID = LoadGameData.Instance.miniGameDatas["game1tree_normal"].GetItemID1;
    
        StartGame();
    }


    void CheckWoodDirection(int input)
    {

        StartCoroutine(SwingAxe());
        if(input == 0) // w
        {
            WoodChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, charDistance);
            WoodChar.transform.localEulerAngles = new Vector3(0,0,180);
        }
        else if(input == 1) //a
        {
            WoodChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-charDistance,0);
            WoodChar.transform.localEulerAngles = new Vector3(0,0,-90);
        }
        else if(input == 2) // s
        {
            WoodChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -charDistance);
            WoodChar.transform.localEulerAngles = new Vector3(0,0,0);
        }
        else if(input == 3) // d
        {
            WoodChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(charDistance,0);
            WoodChar.transform.localEulerAngles = new Vector3(0,0,90);
        }


        if(woodDirection == input)
        {
            audioSource.clip = woodSuccessSound;
            audioSource.Play();
            if(NowWood.CompareTag("MiniGameObj1"))
            {
                AddItem(item1ID,woodNormalRes);
            }
            else if(NowWood.CompareTag("MiniGameObj2"))
            {
                AddItem(item1ID,woodGoldRes);
            }

           Destroy(NowWood);
           SpawnWood();
        }
        else
        {
            audioSource.clip = woodFailSound;
            audioSource.Play();
            StopCoroutine(SwingAxe());
            StartCoroutine(Stun());
        }
    }

    IEnumerator SwingAxe()
    {   IdleObj.SetActive(false);
        AxeObj.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        AxeObj.SetActive(false);
        IdleObj.SetActive(true);
    }

    IEnumerator Stun()
    {
        //AxeObj.SetActive(false);
        StunObj.SetActive(true);
        isStun = true;
        Debug.Log("Wood Stun");
        yield return new WaitForSeconds(woodStunTime);
        AxeObj.SetActive(false);
        StunObj.SetActive(false);
        IdleObj.SetActive(true);
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

        NowWood = Instantiate(wood, transform.position, Quaternion.identity,mother.transform);

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
