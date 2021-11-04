using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroState : MonoBehaviour
{
    public List<Button> heroBtnList = new List<Button>();

    private void Awake()
    {
        for (int i = 0; i < heroBtnList.Count; i++)
        {
            int idx = i;
            heroBtnList[i].onClick.AddListener(() =>
            {
                HeroStateBtn(idx);
            });
        }
    }

    void HeroStateBtn(int _idx)
    {
        //print(_idx);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
