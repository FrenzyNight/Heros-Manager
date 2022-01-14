using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaveManager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;
    
    public GameObject JemPrefab, DownStonePrefab, UpStonePrefab;
    public Vector2 BlockSpawnPoint;
    private Vector2 RealSpawnPoint;
    
    public bool isRun;

    private float standardSpeed = 500f;
    private float standardJump = 1700f;

    private int rnd;

    
    public float realSpeed;
    public float realCharJump;

    //road data
    public float varSpeed;
    public float coolTime;

    public float stunTime;
    public float downStonePer;
    public float upStonePer;
    
    public float jemResource;
    public float jemPer;

    public float charJump;

    //UI

    public float resolutionScale;
    public float heightScale;
    
    private bool isFirst = true;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Test : " + InGameMgr.Instance.miniGameData["game1cage_normal"].speed);
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_1_Forest");
        SetUp();

        StartCoroutine(FirstStart());
        //StartCoroutine("SpawnObject");
    }
    
    void SetUp()
    {
        varSpeed = InGameMgr.Instance.miniGameData["game1cage_normal"].speed;
        coolTime = InGameMgr.Instance.miniGameData["game1cage_normal"].cooltime;
        stunTime = InGameMgr.Instance.miniGameData["game1cage_stone"].stun;
        downStonePer = InGameMgr.Instance.miniGameData["game1cage_stone"].probability * 100;
        upStonePer = InGameMgr.Instance.miniGameData["game1cage_upstone"].probability * 100;
        jemPer = InGameMgr.Instance.miniGameData["game1cage_jem"].probability * 100;
        jemResource = InGameMgr.Instance.miniGameData["game1cage_jem"].jem;
        charJump = InGameMgr.Instance.miniGameData["game1cage_normal"].value1;

        realCharJump = charJump * standardJump;
        realSpeed = varSpeed * standardSpeed;

        resolutionScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;
        RealSpawnPoint = new Vector2(BlockSpawnPoint.x * resolutionScale + transform.position.x , BlockSpawnPoint.y * heightScale + transform.position.y);
        
        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
        
    }

    public void StartGame()
    {
        if(!isFirst)
            StartCoroutine(ReStart());
    }

    void Update()
    {

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

        StartCoroutine(SpawnObject());
    }

    IEnumerator FirstStart()
    {
        //GuidPanel.SetActive(true);
        //GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);

        isFirst = false;
        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        isRun = true;
        
        StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        while(true)
        {
            if(isRun)
            {
                rnd = Random.Range(1,101);

                if(rnd <= jemPer)
                {
                    //JemSpawn
                    Instantiate(JemPrefab, RealSpawnPoint, Quaternion.identity,GameObject.Find("Cave").transform);
                }
                else if(jemPer < rnd && rnd <= jemPer + downStonePer)
                {
                    // DownStoneSpawn
                    Instantiate(DownStonePrefab, RealSpawnPoint, Quaternion.identity, GameObject.Find("Cave").transform);
                }
                else if(jemPer + downStonePer < rnd && rnd <= jemPer + downStonePer + upStonePer)
                {
                    //UpStoneSpawn;
                    Instantiate(UpStonePrefab, RealSpawnPoint, Quaternion.identity, GameObject.Find("Cave").transform);
                }
                else
                {
                    //Debug.Log("rnd : " + rnd.ToString());
                }
            }

            yield return new WaitForSeconds(coolTime);
        }
    }

    public void AddJem()
    {
        MGM.jem += (int)jemResource;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(RealSpawnPoint ,5f);
    }
}
