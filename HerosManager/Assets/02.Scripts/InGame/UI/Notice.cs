using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notice : MonoBehaviour
{
    public Button NoticeBtn;
    public GameObject NoticePannel;

    private void Awake()
    {
        NoticeBtn.onClick.AddListener(() =>
        {
            MovePannel();
        });
    }

    void Start()
    {

    }

    void MovePannel()
    {

    }

    void Update()
    {
        
    }
}
