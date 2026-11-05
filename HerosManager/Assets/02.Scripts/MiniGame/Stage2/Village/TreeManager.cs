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
            else if(MiniGameRules.PercentHit(rnd, treeGoldPer + treeNormalPer))
            {
                SelectedObject = NormalTree;
            }
            else
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
        if (!GamepadUiController.MiniGameInputAllowed || treeList.Count == 0) return;
        if((Input.GetKeyDown(KeyCode.A) || GamepadInput.Pressed(GamepadButton.Left)) && !isStun)
        {
           StartCoroutine(ButtonA());
        }

        else if((Input.GetKeyDown(KeyCode.D) || GamepadInput.Pressed(GamepadButton.Right)) && !isStun)
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
            AddItem(GameData("game2tree_gold").GetItemID1, (int)treeGoldRes);
            GamepadInput.Pulse(0.16f, 0.28f, 0.08f);
        }
        else if(tree.CompareTag("NormalTree"))
        {
            AddItem(GameData("game2tree_norm").GetItemID1, (int)treeNormalRes);
        }
        else if(tree.CompareTag("BugTree"))
        {
            AddItem(GameData("game2tree_norm").GetItemID1, -(int)treeBugLoss);
            GamepadInput.Pulse(0.3f, 0.12f, 0.18f);
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
        else if(MiniGameRules.PercentHit(rnd, treeGoldPer + treeNormalPer))
        {
            SelectedObject = NormalTree;
        }
        else
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
