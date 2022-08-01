using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    public Text mainText;
    public GameObject Fullchar;
    public GameObject fullImgObj;
    public GameObject HalfChar;
    public GameObject topImgObj, botImgObj;
    public GameObject Option;
    public GameObject[] OptionButtons;
    public GameObject OptionResultText;
    
    [HideInInspector]
    public Image topCharImg, botCharImg;
    [HideInInspector]
    public Image fullCharImg;
    [HideInInspector]
    public bool isClicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
