using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MiniGameSetMgr
{
    public Vector2 SpawnPoint1, SpawnPoint2, SpawnPoint3;
    public Vector2 spawnP1, spawnP2, spawnP3;

    public Vector2 TopPoint, BotPoint;
    public float topP, botP;
    public GameObject Log1,Log2;
    public GameObject FishPrefab, GoldFishPrefab;
    private GameObject SelectedFish;

    private LogMove LM1,LM2;

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
        LM1 = Log1.GetComponent<LogMove>();
        LM2 = Log2.GetComponent<LogMove>();

        //SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            LM1.direction *= -1;
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            LM2.direction *= -1;
        }
    }

    public override void SetUp()
    {
        base.SetUp();

        FishPrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        GoldFishPrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;

        fishLogSpeed = LoadGameData.Instance.miniGameDatas["game1log"].Velocity;

        fishCoolTime = LoadGameData.Instance.miniGameDatas["game1fish_normal"].CoolTime;
        fishVarSpeed = LoadGameData.Instance.miniGameDatas["game1fish_normal"].Velocity;

        fishGoldPer = LoadGameData.Instance.miniGameDatas["game1fish_gold"].Probability * 100;
        fishGoldRes = (int)LoadGameData.Instance.miniGameDatas["game1fish_gold"].GetAmount1;

        fishNormalRes = (int)LoadGameData.Instance.miniGameDatas["game1fish_normal"].GetAmount1;

        realFishSpeed = standardFishSpeed * fishVarSpeed;
        realLogSpeed = standardFishSpeed * fishLogSpeed;

        item1ID = LoadGameData.Instance.miniGameDatas["game1fish_normal"].GetItemID1;

        SpawnPoint1 = new Vector2(spawnP1.x * widthScale + transform.position.x, spawnP1.y * heightScale + transform.position.y);
        SpawnPoint2 = new Vector2(spawnP2.x * widthScale + transform.position.x, spawnP2.y * heightScale + transform.position.y);
        SpawnPoint3 = new Vector2(spawnP3.x * widthScale + transform.position.x, spawnP3.y * heightScale + transform.position.y);
    
        TopPoint = new Vector2(transform.position.x, topP * heightScale + transform.position.y);
        BotPoint = new Vector2(transform.position.x, botP * heightScale + transform.position.y);
        
        StartGame();
    }

    public void StartGame()
    {
        base.StartGame();

        foreach(GameObject obj in spawnObjects)
        {
            Destroy(obj);
        }

        spawnObjects.Clear();

        StartCoroutine(FishSpawn());
    }

    IEnumerator FishSpawn()
    {
        while(true)
        {

            rnd = Random.Range(1,101);
            if(rnd <= fishGoldPer)
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
                spawnObjects.Add(Instantiate(SelectedFish, SpawnPoint1, Quaternion.identity, mother.transform));
            }
            else if(rnd == 2)
            {
                spawnObjects.Add(Instantiate(SelectedFish, SpawnPoint2, Quaternion.identity, mother.transform));
            }
            else if(rnd == 3)
            {
                spawnObjects.Add(Instantiate(SelectedFish, SpawnPoint3, Quaternion.identity, mother.transform));
            }

            yield return new WaitForSeconds(fishCoolTime);
        }
    }

    public void GetFish()
    {
        base.AddItem(item1ID, fishNormalRes);
    }

    public void GetGoldFish()
    {
        base.AddItem(item1ID, fishGoldRes);
    }

    void OnDrawGizmos()
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
