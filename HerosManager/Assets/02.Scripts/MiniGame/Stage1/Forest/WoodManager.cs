using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManager : MiniGameSetMgr
{
    AudioSource audioSource;
    public AudioClip woodSuccessSound, woodFailSound;

    public GameObject NormalWoodPrefab, GoldWoodPrefab;
    private GameObject NowWood;
    private Coroutine axeCoroutine;

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
        if(!isStun && GamepadUiController.GameplayInputAllowed)
        {
            if(Input.GetKeyDown(KeyCode.W) || GamepadInput.Pressed(GamepadButton.Up))
            {
                CheckWoodDirection(0);
            }
            if(Input.GetKeyDown(KeyCode.A) || GamepadInput.Pressed(GamepadButton.Left))
            {
                CheckWoodDirection(1);
            }
            if(Input.GetKeyDown(KeyCode.S) || GamepadInput.Pressed(GamepadButton.Down))
            {
                CheckWoodDirection(2);
            }
            if(Input.GetKeyDown(KeyCode.D) || GamepadInput.Pressed(GamepadButton.Right))
            {
                CheckWoodDirection(3);
            }
        }
    }

    public override void SetUp()
    {
        base.SetUp();
        isStun = false;
        axeCoroutine = null;
        StunObj.SetActive(false);
        AxeObj.SetActive(false);
        IdleObj.SetActive(true);
        Destroy(NowWood);

        audioSource = gameObject.GetComponent<AudioSource>();

        woodStunTime = GameData("game1tree_normal").Stun;

        woodGoldRes = (int)GameData("game1tree_gold").GetAmount1;
        woodGoldPer = GameData("game1tree_gold").Probability * 100;

        woodNormalRes = (int)GameData("game1tree_normal").GetAmount1;

        item1ID = GameData("game1tree_normal").GetItemID1;
    
        StartGame();
    }


    void CheckWoodDirection(int input)
    {

        if (axeCoroutine != null)
            StopCoroutine(axeCoroutine);
        axeCoroutine = StartCoroutine(SwingAxe());
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
            GamepadInput.Pulse(0.15f, 0.3f, 0.08f);
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
            GamepadInput.Pulse(0.6f, 0.15f, 0.18f);
            audioSource.Play();
            if (axeCoroutine != null)
                StopCoroutine(axeCoroutine);
            axeCoroutine = null;
            StartCoroutine(Stun());
        }
    }

    IEnumerator SwingAxe()
    {   IdleObj.SetActive(false);
        AxeObj.SetActive(true);
        yield return new WaitForSeconds(MiniGameRules.PositiveDelay(0.3f));
        AxeObj.SetActive(false);
        IdleObj.SetActive(true);
        axeCoroutine = null;
    }

    IEnumerator Stun()
    {
        //AxeObj.SetActive(false);
        StunObj.SetActive(true);
        isStun = true;
        Debug.Log("Wood Stun");
        yield return new WaitForSeconds(MiniGameRules.PositiveDelay(woodStunTime));
        AxeObj.SetActive(false);
        StunObj.SetActive(false);
        IdleObj.SetActive(true);
        isStun = false;
    }

    void SpawnWood()
    {
        int rnd = Random.Range(1,101);
        GameObject wood;

        if(MiniGameRules.PercentHit(rnd, woodGoldPer))
        {
            wood = GoldWoodPrefab;
        }
        else
        {
            wood = NormalWoodPrefab;
        }

        NowWood = Spawn(wood, transform.position, Quaternion.identity);

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
