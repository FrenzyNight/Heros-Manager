using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;
    private bool isFisrt = true;

    public Vector2 SpawnPoint1, SpawnPoint2, SpawnPoint3;
    public Vector2 spawnP1, spawnP2, spawnP3;

    public Vector2 TopPoint, BotPoint;
    public float topP, botP;
    public GameObject Log1,Log2;
    public GameObject FishPrefab, GoldFishPrefab;
    private GameObject SelectedFish;

    private LogMove LM1,LM2;

    private float x, y;
    private int rnd;

    //read

    public float standardFishSpeed = 100f;

    public float fishLogSpeed;

    public float fishCoolTime;
    public float fishVarSpeed;

    public float fishGoldPer;
    public float fishGoldRes;

    public float fishNormalRes;

    // real
    public float realFishSpeed;
    public float realLogSpeed;

    //resolution

    public float widthScale, heightScale;
    // Start is called before the first frame update
    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_1_river");
        LM1 = Log1.GetComponent<LogMove>();
        LM2 = Log2.GetComponent<LogMove>();

        SetUp();
        StartCoroutine(FirstStart());
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

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        fishLogSpeed = InGameMgr.Instance.miniGameData["game1log"].speed;

        fishCoolTime = InGameMgr.Instance.miniGameData["game1fish_normal"].cooltime;
        fishVarSpeed = InGameMgr.Instance.miniGameData["game1fish_normal"].speed;

        fishGoldPer = InGameMgr.Instance.miniGameData["game1fish_gold"].probability * 100;
        fishGoldRes = InGameMgr.Instance.miniGameData["game1fish_gold"].meat;

        fishNormalRes = InGameMgr.Instance.miniGameData["game1fish_normal"].meat;

        realFishSpeed = standardFishSpeed * fishVarSpeed;
        realLogSpeed = standardFishSpeed * fishLogSpeed;

        SpawnPoint1 = new Vector2(spawnP1.x * widthScale + transform.position.x, spawnP1.y * heightScale + transform.position.y);
        SpawnPoint2 = new Vector2(spawnP2.x * widthScale + transform.position.x, spawnP2.y * heightScale + transform.position.y);
        SpawnPoint3 = new Vector2(spawnP3.x * widthScale + transform.position.x, spawnP3.y * heightScale + transform.position.y);
    
        TopPoint = new Vector2(transform.position.x, topP * heightScale + transform.position.y);
        BotPoint = new Vector2(transform.position.x, botP * heightScale + transform.position.y);
    
    
        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    IEnumerator FirstStart()
    {
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        isFisrt = false;
        StartCoroutine(FishSpawn());
    }

    public void StartGame()
    {
        if(!isFisrt)
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
                Instantiate(SelectedFish, SpawnPoint1, Quaternion.identity, GameObject.Find("Fish").transform);
            }
            else if(rnd == 2)
            {
                Instantiate(SelectedFish, SpawnPoint2, Quaternion.identity, GameObject.Find("Fish").transform);
            }
            else if(rnd == 3)
            {
                Instantiate(SelectedFish, SpawnPoint3, Quaternion.identity, GameObject.Find("Fish").transform);
            }

            yield return new WaitForSeconds(fishCoolTime);
        }
    }

    public void GetFish()
    {
        MGM.meat += (int)fishNormalRes;
    }

    public void GetGoldFish()
    {
        MGM.meat += (int)fishGoldRes;
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
