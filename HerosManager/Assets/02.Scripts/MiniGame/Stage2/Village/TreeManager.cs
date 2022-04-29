using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MiniGameSetMgr
{

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

    private bool isStun;


    void Start()
    {
        SetUp();
    }

    void SetUp()
    {

        treeInputDelay = LoadGameData.Instance.miniGameDatas["game2tree_norm"].Stun;

        treeNormalPer = LoadGameData.Instance.miniGameDatas["game2tree_norm"].Probability * 100;
        treeNormalRes = LoadGameData.Instance.miniGameDatas["game2tree_norm"].GetAmount1;

        treeGoldRes = LoadGameData.Instance.miniGameDatas["game2tree_gold"].GetAmount1;
        treeGoldPer = LoadGameData.Instance.miniGameDatas["game2tree_gold"].Probability * 100;

        treeBugLoss = LoadGameData.Instance.miniGameDatas["game2tree_insect"].value1;
        treeBugPer = LoadGameData.Instance.miniGameDatas["game2tree_insect"].Probability * 100;
        treeBugStun = LoadGameData.Instance.miniGameDatas["game2tree_insect"].Stun;

        
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
    }

    
}
