using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventMgr : Singleton<EventMgr>
{
    public GameObject OptionButtonPrefab;
    public Text mainText;
    public GameObject Fullchar;
    public GameObject HalfChar;
    public GameObject Option;
    public Sprite[] FullChars;
    public Sprite[] TopChars;
    public Sprite[] BotChars;

    [HideInInspector]
    public List<string> EventList = new List<string>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUp()
    {
        foreach(var evt in LoadGameData.Instance.eventDatas)
        {
            if(evt.OpenType == 1)
            {
                EventList.Add(evt.EventID);
            }
        }
    }
}
