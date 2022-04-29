using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpringManager : MiniGameSetMgr
{
    public GameObject Command;
    public GameObject[] Altars;

    //road

    public float msSpringRes;
    public float msSpringWaterTime;
    public float msSpringGraceTime;
    public float msSpringJewelTime;

    public float msAlterMinTime;
    public float msAlterMaxTime;

    public float msKeyPadNum;
    public float msKeyPadStun;

    //check
    public bool[] isAltar;
    public bool isSpring;
    public bool isCommand;

    // Start is called before the first frame update
    void Start()
    {
        
        SetUp();
        isCommand = false;
    }

    void SetUp()
    {

        msSpringRes = LoadGameData.Instance.miniGameDatas["game3MagicWell"].GetAmount1;
        msSpringWaterTime = LoadGameData.Instance.miniGameDatas["game3MagicWell"].value1;
        msSpringGraceTime = LoadGameData.Instance.miniGameDatas["game3MagicWell"].value3;
        msSpringJewelTime = LoadGameData.Instance.miniGameDatas["game3MagicWell"].value2;

        msAlterMinTime = LoadGameData.Instance.miniGameDatas["game3Altar1"].value1;
        msAlterMaxTime = LoadGameData.Instance.miniGameDatas["game3Altar1"].value2;

        msKeyPadNum = LoadGameData.Instance.miniGameDatas["game3KeyPad"].value3;
        msKeyPadStun = LoadGameData.Instance.miniGameDatas["game3KeyPad"].Stun;

    }

    public void StartGame()
    {
        
    }


    public void GetWater()
    {
        //MGM.water += (int)msSpringRes;
    }

    public void CommandActive(int n)
    {
        isCommand = true;
        Command.SetActive(true);
        Command.GetComponent<MagicSpringCommand>().SetUp(n);
    }

    public void AltarActive(int n)
    {
        isCommand = false;
        isAltar[n] = true;
        Altars[n].GetComponent<MagicSpringAltar>().Active();
    }

    public void AltarDeActive(int n)
    {
        isAltar[n] = false;
    }
}
