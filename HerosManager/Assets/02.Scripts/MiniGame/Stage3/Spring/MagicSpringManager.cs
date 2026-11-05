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
        
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();
        isCommand = false;

        msSpringRes = GameData("game3MagicWell").GetAmount1;
        msSpringWaterTime = GameData("game3MagicWell").value1;
        msSpringGraceTime = GameData("game3MagicWell").value3;
        msSpringJewelTime = GameData("game3MagicWell").value2;

        msAlterMinTime = GameData("game3Altar1").value1;
        msAlterMaxTime = GameData("game3Altar1").value2;

        msKeyPadNum = GameData("game3KeyPad").value3;
        msKeyPadStun = GameData("game3KeyPad").Stun;

    }

    public override void StartGame()
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
