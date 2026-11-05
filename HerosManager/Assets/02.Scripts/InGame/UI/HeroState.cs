using System.Collections;
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

        var table = LoadGameData.Instance.heroStateDatas;
        string prefix = _heroData.HeroStateGroupID + "_";
        var heroStateDataList = new[]
        {
            table[prefix + "Stress"], table[prefix + "Power"],
            table[prefix + "Hp"], table[prefix + "Exp"]
        };

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
        ExpFill.fillAmount = maxExp > 0f ? exp / maxExp : 0f;
        SetStatText();
    }

    public float[] CaptureStats()
    {
        return new[] { stress, power, hp, exp };
    }

    public void RestoreStats(float[] values)
    {
        if (values == null || values.Length != 4)
            return;
        for (int i = 0; i < values.Length; i++)
            if (float.IsNaN(values[i]) || float.IsInfinity(values[i]))
                return;
        SetStats(values[0], values[1], values[2], values[3]);
    }

    void SetStats(float nextStress, float nextPower, float nextHp, float nextExp)
    {
        stress = Mathf.Clamp(nextStress, minStress, maxStress);
        power = Mathf.Clamp(nextPower, minPower, maxPower);
        hp = Mathf.Clamp(nextHp, minHp, maxHp);
        exp = Mathf.Clamp(nextExp, minExp, maxExp);
        ExpFill.fillAmount = maxExp > 0f ? exp / maxExp : 0f;
        SetStatText();
    }

    public void AddStat(float _stress, float _power, float _hp, float _exp)
    {
        SetStats(stress + _stress, power + _power, hp + _hp, exp + _exp);
    }

    public void AddStat(string statCode ,float amount)
    {
        switch (statCode)
        {
            case "stress": AddStat(amount, 0, 0, 0); break;
            case "power": AddStat(0, amount, 0, 0); break;
            case "hp": AddStat(0, 0, amount, 0); break;
            case "exp": AddStat(0, 0, 0, amount); break;
        }
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
