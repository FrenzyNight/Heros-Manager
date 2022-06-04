﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HeroState : MonoBehaviour
{
    public float stress;
    public float power;
    public float hp;
    public float exp;

    float minStress;
    float maxStress;
    float minPower;
    float maxPower;
    float minHp;
    float maxHp;
    float minExp;
    float maxExp;

    string stressStr;
    string powerStr;
    string hpStr;

    public Text NameText;
    public Text JobText;
    public TextMeshProUGUI StateText;
    public Text ExpText;
    public Image ExpFill;

    public Button button;

    public bool isSlide;

    void Start()
    {
        isSlide = false;

        button.onClick.AddListener(() => { Slide(); });
    }

    public void Setup(HeroData _heroData)
    {
        NameText.text = LoadGameData.Instance.GetString(_heroData.HeroStringID);
        //JobText.text = LoadGameData.Instance.GetString(_heroData.HeroStringID);

        List<HeroStateData> heroStateDataList = new List<HeroStateData>();
        foreach (var data in LoadGameData.Instance.heroStateDatas)
        {
            if (data.Value.HeroStateGroupID == _heroData.HeroStateGroupID)
            {
                heroStateDataList.Add(data.Value);
            }
        }

        //0.Stress 1.Power 2.Hp 3.Exp
        stress = heroStateDataList[0].FirstGive;
        power = heroStateDataList[1].FirstGive;
        hp = heroStateDataList[2].FirstGive;
        exp = heroStateDataList[3].FirstGive;

        minStress = heroStateDataList[0].Min;
        maxStress = heroStateDataList[0].Max;
        minPower = heroStateDataList[1].Min;
        maxPower = heroStateDataList[1].Max;
        minHp = heroStateDataList[2].Min;
        maxHp = heroStateDataList[2].Max;
        minExp = heroStateDataList[3].Min;
        maxExp = heroStateDataList[3].Max;

        stressStr = LoadGameData.Instance.GetString(heroStateDataList[0].HeroStateStringID);
        powerStr = LoadGameData.Instance.GetString(heroStateDataList[1].HeroStateStringID);
        hpStr = LoadGameData.Instance.GetString(heroStateDataList[2].HeroStateStringID);

        ExpText.text = LoadGameData.Instance.GetString(heroStateDataList[3].HeroStateStringID);
        ExpFill.fillAmount = exp / maxExp;
        SetStatText();
    }

    public void AddStat(float _stress, float _power, float _hp, float _exp)
    {
        stress += _stress;
        if (stress > maxStress)
            stress = maxStress;
        if (stress < minStress)
            stress = minStress;

        power += _power;
        if (power > maxPower)
            power = maxPower;
        if (power < minPower)
            power = minPower;

        hp += _hp;
        if (hp > maxHp)
            hp = maxHp;
        if (hp < minHp)
            hp = minHp;

        exp += _exp;
        if (exp > maxExp)
            exp = maxExp;
        if (exp < minExp)
            exp = minExp;


        ExpFill.fillAmount = exp / maxExp;
        SetStatText();
    }

    public void AddStat(string statCode ,float amount)
    {
        if(statCode == "stress")
        {
            stress += amount;
            if (stress > maxStress)
                stress = maxStress;
            if (stress < minStress)
                stress = minStress;
        }
        
        else if(statCode == "power")
        {
            power += amount;
            if (power > maxPower)
                power = maxPower;
            if (power < minPower)
                power = minPower;
        }

        else if(statCode == "hp")
        {
            hp += amount;
            if (hp > maxHp)
                hp = maxHp;
            if (hp < minHp)
                hp = minHp;
        }

        else if(statCode == "exp")
        {
            exp += amount;
            if (exp > maxExp)
                exp = maxExp;
            if (exp < minExp)
                exp = minExp;
        }


        ExpFill.fillAmount = exp / maxExp;
        SetStatText();
    }

    void SetStatText()
    {
        string str = stressStr;
        if (stress <= 30)
            str += LoadGameData.Instance.GetString("HeroState_a1");
        else if (stress <= 70)
            str += LoadGameData.Instance.GetString("HeroState_a2");
        else if (stress <= 100)
            str += LoadGameData.Instance.GetString("HeroState_a3");

        str += "\n" + string.Format(powerStr, power) + "\n" + string.Format(hpStr, hp, maxHp);

        StateText.text = str;
    }

    public void Slide()
    {
        if (isSlide)
            this.transform.DOLocalMoveX(30, 1f).SetUpdate(true).SetEase(Ease.OutQuad);
        else
            this.transform.DOLocalMoveX(-121, 1f).SetUpdate(true).SetEase(Ease.OutQuad);

        isSlide = !isSlide;
    }
}