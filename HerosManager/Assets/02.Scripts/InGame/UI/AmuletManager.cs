using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AmuletManager : Singleton<AmuletManager>
{
    public GameObject AmuletNotice;
    public Text NoticeText;

    public GameObject AmuletPrefab;

    public Button JewelButton;
    public GameObject JewelList;
    public bool isSlide;

    public List<GameObject> JewelInven = new List<GameObject>();
    public List<string> jemIDList = new List<string>();

    //public bool isMouseOn = false;
    
    void Start()
    {
        Setup();
    }

    void Setup()
    {
        isSlide = true;
        JewelListSlide();
    }

    public void GetAmulet(string jemID)
    {
        if (jemIDList.Contains(jemID))
        {
            //LevelUp
            //
            foreach (GameObject var in JewelInven)
            {
                if (var.GetComponent<Amulet>().JemID == jemID)
                {
                    var.GetComponent<Amulet>().LevelUp();
                    break;
                }
            }
        }
        else
        {
            //Swpan
            SpawnAmulet(jemID);
            jemIDList.Add(jemID);
        }
    }

    void SpawnAmulet(string jemID)
    {
        GameObject obj = Instantiate(AmuletPrefab, this.transform);
        obj.GetComponent<Amulet>().Setup(jemID);
        JewelInven.Add(obj);
    }

    public void JewelListSlide()
    {
        if(isSlide)
        {
            JewelList.GetComponent<RectTransform>().DOAnchorPosX(-485, 1f).SetUpdate(true).SetEase(Ease.OutQuad);
        }
        else
        {
            JewelList.GetComponent<RectTransform>().DOAnchorPosX(485, 1f).SetUpdate(true).SetEase(Ease.OutQuad);
        }
        isSlide = !isSlide;
    }

    public void CloseList()
    {
        isSlide = true;
        JewelListSlide();
    }
}
