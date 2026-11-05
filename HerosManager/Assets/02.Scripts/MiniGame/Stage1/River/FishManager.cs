using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MiniGameSetMgr
{
    AudioSource audioSource;

    public Vector2 SpawnPoint1, SpawnPoint2, SpawnPoint3;
    public Vector2 spawnP1, spawnP2, spawnP3;

    public Vector2 TopPoint, BotPoint;
    public float topP, botP;
    public GameObject Log1;
    public GameObject FishPrefab, GoldFishPrefab;
    private GameObject SelectedFish;

    private LogMove LM1;

    private float x, y;

    //read

    public float standardFishSpeed = 100f;

    public float fishLogSpeed;

    public float fishCoolTime;
    public float fishVarSpeed;

    public float fishGoldPer;
    public int fishGoldRes;

    public int fishNormalRes;

    // real
    public float realFishSpeed;
    public float realLogSpeed;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        LM1 = Log1.GetComponent<LogMove>();

        //SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            LM1.direction *= -1;
        }

        /*
        if(Input.GetKeyDown(KeyCode.D))
        {
            LM2.direction *= -1;
        }
        */
    }

    public override void SetUp()
    {
        base.SetUp();
        audioSource = GetComponent<AudioSource>();
        LM1 = Log1.GetComponent<LogMove>();


        fishLogSpeed = GameData("game1log").Velocity;

        fishCoolTime = GameData("game1fish_normal").CoolTime;
        fishVarSpeed = GameData("game1fish_normal").Velocity;

        fishGoldPer = GameData("game1fish_gold").Probability * 100;
        fishGoldRes = (int)GameData("game1fish_gold").GetAmount1;

        fishNormalRes = (int)GameData("game1fish_normal").GetAmount1;

        realFishSpeed = standardFishSpeed * fishVarSpeed;
        realLogSpeed = standardFishSpeed * fishLogSpeed;

        item1ID = GameData("game1fish_normal").GetItemID1;

        SpawnPoint1 = ScreenPoint(spawnP1);
        SpawnPoint2 = ScreenPoint(spawnP2);
        SpawnPoint3 = ScreenPoint(spawnP3);
    
        TopPoint = new Vector2(transform.position.x, topP * heightScale + transform.position.y);
        BotPoint = new Vector2(transform.position.x, botP * heightScale + transform.position.y);
        
        StartGame();
    }

    public override void StartGame()
    {
        base.StartGame();


        StartCoroutine(FishSpawn());
    }

    IEnumerator FishSpawn()
    {
        while(true)
        {

            rnd = Random.Range(1,101);
            if(MiniGameRules.PercentHit(rnd, fishGoldPer))
            {
                SelectedFish = GoldFishPrefab;
            }
            else
            {
                SelectedFish = FishPrefab;
            }
            
            rnd = Random.Range(1,4);

            if(rnd == 1)
            {
                Spawn(SelectedFish, SpawnPoint1, Quaternion.identity);
            }
            else if(rnd == 2)
            {
                Spawn(SelectedFish, SpawnPoint2, Quaternion.identity);
            }
            else if(rnd == 3)
            {
                Spawn(SelectedFish, SpawnPoint3, Quaternion.identity);
            }

            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(fishCoolTime));
        }
    }

    public void GetFish()
    {
        audioSource.Play();
        base.AddItem(item1ID, fishNormalRes);
    }

    public void GetGoldFish()
    {
        audioSource.Play();
        base.AddItem(item1ID, fishGoldRes);
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint1, 3f);
        Gizmos.DrawSphere(SpawnPoint2, 3f);
        Gizmos.DrawSphere(SpawnPoint3, 3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(TopPoint, 3f);
        Gizmos.DrawSphere(BotPoint, 3f);
    }
}
