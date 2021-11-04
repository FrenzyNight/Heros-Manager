using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collect : MonoBehaviour
{
    public Button CollectBtn;

    private void Awake()
    {
        CollectBtn.onClick.AddListener(() =>
        {
            InGameMgr.Instance.state = InGameMgr.State.Pannel;
        });
    }

    //void Start()
    //{

    //}

    //void Update()
    //{

    //}
}
