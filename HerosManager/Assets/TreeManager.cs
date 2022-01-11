﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject GoldTree, NormalTree, BugTree;

    //private Vector2 SpawnPoint;
    //public Vector2 point;

    private GameObject SelectedObject;
    
    List<GameObject> treeList = new List<GameObject>();

    //load

    public float treeNormalRes;
    public float treeNormalPer;

    public float treeGoldRes;
    public float treeGoldPer;
    
    public float treeBugLoss;
    public float treeBugPer;

    //real
    private int rnd;
    //scale
    public float widthScale, heightScale;

    void Start()
    {
        InGameMgr.Instance.EnterMiniGame("Stage_2_ruins");
        SetUp();
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        treeNormalPer = InGameMgr.Instance.miniGameData["game2tree_norm"].probability * 100;
        treeNormalRes = InGameMgr.Instance.miniGameData["game2tree_norm"].wood;

        treeGoldRes = InGameMgr.Instance.miniGameData["game2tree_gold"].wood;
        treeGoldPer = InGameMgr.Instance.miniGameData["game2tree_gold"].probability * 100;

        treeBugLoss = InGameMgr.Instance.miniGameData["game2tree_insect"].value1;
        treeBugPer = InGameMgr.Instance.miniGameData["game2tree_insect"].probability * 100;

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
            tree.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 192);
            treeList.Add(tree);

            for(int j=0;j<=i-1;j++)
            {
                treeList[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, treeList[j].GetComponent<RectTransform>().anchoredPosition.y - 128); 
            }

        }
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            ButtonA();
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            ButtonD();
        }
    }

    void ButtonA()
    {
        GameObject tree = treeList[0];
        treeList.RemoveAt(0);
        Destroy(tree);

        SpanwNewTree();
    }

    void ButtonD()
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
            Debug.Log("Loss tree : " + treeBugLoss.ToString());
        }
        else
        {
            Debug.Log("error");
        }

        treeList.RemoveAt(0);
        Destroy(tree);

        SpanwNewTree();
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
        tree.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 192);
        treeList.Add(tree);

        for(int j=0;j<3;j++)
        {
            treeList[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, treeList[j].GetComponent<RectTransform>().anchoredPosition.y - 128); 
        }
    }
/*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint, 3f);
    }
    */
}
