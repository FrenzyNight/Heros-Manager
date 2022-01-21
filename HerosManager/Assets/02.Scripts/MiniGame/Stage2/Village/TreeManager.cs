using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;

    public GameObject GoldTree, NormalTree, BugTree;

    //private Vector2 SpawnPoint;
    //public Vector2 point;

    private GameObject SelectedObject;
    
    List<GameObject> treeList = new List<GameObject>();

    //load
    public float treeInputDelay;
    public float treeNormalRes;
    public float treeNormalPer;

    public float treeGoldRes;
    public float treeGoldPer;
    
    public float treeBugLoss;
    public float treeBugPer;
    public float treeBugStun;

    //real
    private int rnd;
    //scale
    public float widthScale, heightScale;

    private bool isStun;

    //UI
    private bool isFirst = true;

    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_2_ruins");
        
        SetUp();
        StartCoroutine(FirstStart());
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        treeInputDelay = InGameMgr.Instance.miniGameData["game2tree_norm"].stun;

        treeNormalPer = InGameMgr.Instance.miniGameData["game2tree_norm"].probability * 100;
        treeNormalRes = InGameMgr.Instance.miniGameData["game2tree_norm"].wood;

        treeGoldRes = InGameMgr.Instance.miniGameData["game2tree_gold"].wood;
        treeGoldPer = InGameMgr.Instance.miniGameData["game2tree_gold"].probability * 100;

        treeBugLoss = InGameMgr.Instance.miniGameData["game2tree_insect"].value1;
        treeBugPer = InGameMgr.Instance.miniGameData["game2tree_insect"].probability * 100;
        treeBugStun = InGameMgr.Instance.miniGameData["game2tree_insect"].stun;

        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
        //SpawnPoint = new Vector2(point.x * widthScale + transform.position.x, point.y * heightScale + transform.position.y);

        for(int i=0;i<4;i++)
        {
            rnd = Random.Range(1,101);

            if(rnd <= treeGoldPer)
            {
                SelectedObject = GoldTree;
            }
            else if(treeGoldPer < rnd && rnd <= treeNormalPer)
            {
                SelectedObject = NormalTree;
            }
            else if(treeNormalPer < rnd)
            {
                SelectedObject = BugTree;
            }

            GameObject tree = Instantiate(SelectedObject, transform.position, Quaternion.identity, GameObject.Find("Tree").transform);
            tree.transform.SetAsFirstSibling();
            //tree.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 192);
            treeList.Add(tree);

            for(int j=0;j<=i-1;j++)
            {
                treeList[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, treeList[j].GetComponent<RectTransform>().anchoredPosition.y - 64); 
            }

        }
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) && !isStun)
        {
           StartCoroutine(ButtonA());
        }

        if(Input.GetKeyDown(KeyCode.D) && !isStun)
        {
            StartCoroutine(ButtonD());
        }
    }

    IEnumerator ButtonA()
    {
        GameObject tree = treeList[0];
        treeList.RemoveAt(0);
        Destroy(tree);

        SpanwNewTree();
        isStun = true;
        yield return new WaitForSeconds(treeInputDelay);
        isStun = false;
    }

    IEnumerator ButtonD()
    {
        GameObject tree = treeList[0];

        if(tree.CompareTag("GoldTree"))
        {
            Debug.Log("Get gold tree : " + treeGoldRes.ToString());
        }
        else if(tree.CompareTag("NormalTree"))
        {
            Debug.Log("Get tree : " + treeNormalRes.ToString());
        }
        else if(tree.CompareTag("BugTree"))
        {
            Debug.Log("Loss tree : " + treeBugLoss.ToString() + "and Stun");
            isStun = true;
            yield return new WaitForSeconds(treeBugStun);
            isStun = false;
        }
        else
        {
            Debug.Log("error");
        }

        treeList.RemoveAt(0);
        Destroy(tree);

        SpanwNewTree();

        isStun = true;
        yield return new WaitForSeconds(treeInputDelay);
        isStun = false;
    }

    void SpanwNewTree()
    {
        rnd = Random.Range(1,101);

        if(rnd <= treeGoldPer)
        {
            SelectedObject = GoldTree;
        }
        else if(treeGoldPer < rnd && rnd <= treeNormalPer)
        {
            SelectedObject = NormalTree;
        }
        else if(treeNormalPer < rnd)
        {
            SelectedObject = BugTree;
        }

        GameObject tree = Instantiate(SelectedObject, transform.position, Quaternion.identity, GameObject.Find("Tree").transform);
        tree.transform.SetAsFirstSibling();
        //tree.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 192);
        treeList.Add(tree);

        for(int j=0;j<3;j++)
        {
            treeList[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, treeList[j].GetComponent<RectTransform>().anchoredPosition.y - 64); 
        }
    }

    public void StartGame()
    {
        if(!isFirst)
            StartCoroutine(ReStart());
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

        
    }
}
