using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    void Start()
    {
        LoadGameData.Instance.LoadCSVDatas();
        SceneManager.LoadScene("InGame");
    }

    //void Update()
    //{
        
    //}
}
