using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameMgr : Singleton<MiniGameMgr>
{
    public GameObject MiniGamePanel;

    public Button ExitBtn;
    public Text[] ItemsText;
    public Transform MiniGameTrans;

    GameObject currentMiniGame;

    //temp
    Dictionary<string, GameObject> MiniGameObjs;

    [Header("TempItem")]
    public int wood;
    public int water;
    public int meat;
    public int hub;
    public int food;

    void Start()
    {
        ExitBtn.onClick.AddListener(CloseMiniGame);


    }

    public void Setup()
    {
        InGameMgr.Instance.state = State.MiniGame;

        MiniGamePanel.SetActive(true);

        MiniGameObjs[""].SetActive(true);

        wood = 0;
        water = 0;
        meat = 0;
        hub = 0;
        food = 0;

        currentMiniGame = MiniGameObjs[""];
    }

    public void AddTempItem(int _wood, int _water, int _meat, int _hub, int _food)
    {
        wood += _wood;
        water += _water;
        meat += _meat;
        hub += _hub;
        food += _food;

        ItemsText[0].text = wood.ToString();
        ItemsText[1].text = water.ToString();
        ItemsText[2].text = meat.ToString();
        ItemsText[3].text = hub.ToString();
        ItemsText[4].text = food.ToString();
    }

    public void CloseMiniGame()
    {
        InGameMgr.Instance.state = State.Camp;

        currentMiniGame.SetActive(false);
        MiniGamePanel.SetActive(false);

        ItemManager.Instance.AddItem(wood, water, meat, hub, food);
    }
}
