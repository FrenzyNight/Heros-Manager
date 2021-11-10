using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pannel : MonoBehaviour
{
    public GameObject PannelBack;
    public GameObject CollectPannel;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (InGameMgr.Instance.state == State.Pannel)
            {
                CloseCollectPannel();
                InGameMgr.Instance.state = State.Camp;
            }
        }
    }

    public void OpenCollectPannel()
    {
        PannelBack.SetActive(true);
        CollectPannel.SetActive(true);
    }

    public void CloseCollectPannel()
    {
        PannelBack.SetActive(false);
        CollectPannel.SetActive(false);
    }
}
