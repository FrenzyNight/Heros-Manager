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
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();
        treeList.Clear();
        isStun = false;

        treeInputDelay = GameData("game2tree_norm").Stun;

        treeNormalPer = GameData("game2tree_norm").Probability * 100;
        treeNormalRes = GameData("game2tree_norm").GetAmount1;

        treeGoldRes = GameData("game2tree_gold").GetAmount1;
        treeGoldPer = GameData("game2tree_gold").Probability * 100;

        treeBugLoss = GameData("game2tree_insect").value1;
        treeBugPer = GameData("game2tree_insect").Probability * 100;
        treeBugStun = GameData("game2tree_insect").Stun;

        
        for(int i=0;i<4;i++)
        {
            rnd = Random.Range(1,101);

            if(MiniGameRules.PercentHit(rnd, treeGoldPer))
            {
                SelectedObject = GoldTree;
            }
            else if(treeGoldPer < rnd && MiniGameRules.PercentHit(rnd, treeNormalPer))
            {
                SelectedObject = NormalTree;
            }
            else if(treeNormalPer < rnd)
            {
                SelectedObject = BugTree;
            }

            GameObject tree = Spawn(SelectedObject, transform.position, Quaternion.identity, SpawnContainer("Tree"));
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
        yield return new WaitForSeconds(MiniGameRules.PositiveDelay(treeInputDelay));
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
            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(treeBugStun));
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
        yield return new WaitForSeconds(MiniGameRules.PositiveDelay(treeInputDelay));
        isStun = false;
    }

    void SpanwNewTree()
    {
        rnd = Random.Range(1,101);

        if(MiniGameRules.PercentHit(rnd, treeGoldPer))
        {
            SelectedObject = GoldTree;
        }
        else if(treeGoldPer < rnd && MiniGameRules.PercentHit(rnd, treeNormalPer))
        {
            SelectedObject = NormalTree;
        }
        else if(treeNormalPer < rnd)
        {
            SelectedObject = BugTree;
        }

        GameObject tree = Spawn(SelectedObject, transform.position, Quaternion.identity, SpawnContainer("Tree"));
        tree.transform.SetAsFirstSibling();
        //tree.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 192);
        treeList.Add(tree);

        for(int j=0;j<3;j++)
        {
            treeList[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, treeList[j].GetComponent<RectTransform>().anchoredPosition.y - 64); 
        }
    }

    public override void StartGame()
    {
    }

    
}
