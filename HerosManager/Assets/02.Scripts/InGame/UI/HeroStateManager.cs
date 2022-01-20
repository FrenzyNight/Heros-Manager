using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStateManager : Singleton<HeroStateManager>
{
    public HeroInfo[] heroInfos = new HeroInfo[4];
    HeroState[] heroStateList;

    void Start()
    {
        heroStateList = this.GetComponentsInChildren<HeroState>();

        Setup();
    }

    void Setup()
    {
        for (int i = 0; i < heroInfos.Length; i++)
        {
            string hpKey = "Heros" + (i + 1) + "Hp";
            string powerKey = "Heros" + (i + 1) + "Power";
            string stressKey = "Heros" + (i + 1) + "Stress";
            string expKey = "Heros" + (i + 1) + "Exp";
            HeroInfo hInfo = new HeroInfo(LoadGameData.Instance.herosDatas[hpKey].startState
                , LoadGameData.Instance.herosDatas[powerKey].startState
                , LoadGameData.Instance.herosDatas[stressKey].startState
                , LoadGameData.Instance.herosDatas[expKey].startState);
            heroInfos[i] = hInfo;
            heroStateList[i].SetState(hInfo);
        }
    }

    public void SetJourneyState(int _case)
    {
        switch (_case)
        {
            case 1:
                for (int i = 0; i < heroInfos.Length; i++)
                {
                    string hpKey = "Heros" + (i + 1) + "Hp";
                    string powerKey = "Heros" + (i + 1) + "Power";
                    string stressKey = "Heros" + (i + 1) + "Stress";
                    string expKey = "Heros" + (i + 1) + "Exp";

                    heroInfos[i].hp += LoadGameData.Instance.herosDatas[hpKey].journeyPerfect;
                    heroInfos[i].power += LoadGameData.Instance.herosDatas[powerKey].journeyPerfect;
                    heroInfos[i].stress += LoadGameData.Instance.herosDatas[stressKey].journeyPerfect;
                    heroInfos[i].exp += LoadGameData.Instance.herosDatas[expKey].journeyPerfect;
                    heroStateList[i].SetState(heroInfos[i]);
                }
                break;
            case 2:
                for (int i = 0; i < heroInfos.Length; i++)
                {
                    string hpKey = "Heros" + (i + 1) + "Hp";
                    string powerKey = "Heros" + (i + 1) + "Power";
                    string stressKey = "Heros" + (i + 1) + "Stress";
                    string expKey = "Heros" + (i + 1) + "Exp";

                    heroInfos[i].hp += LoadGameData.Instance.herosDatas[hpKey].journeySuccess;
                    heroInfos[i].power += LoadGameData.Instance.herosDatas[powerKey].journeySuccess;
                    heroInfos[i].stress += LoadGameData.Instance.herosDatas[stressKey].journeySuccess;
                    heroInfos[i].exp += LoadGameData.Instance.herosDatas[expKey].journeySuccess;
                    heroStateList[i].SetState(heroInfos[i]);
                }
                break;
            case 3:
                for (int i = 0; i < heroInfos.Length; i++)
                {
                    string hpKey = "Heros" + (i + 1) + "Hp";
                    string powerKey = "Heros" + (i + 1) + "Power";
                    string stressKey = "Heros" + (i + 1) + "Stress";
                    string expKey = "Heros" + (i + 1) + "Exp";

                    heroInfos[i].hp += LoadGameData.Instance.herosDatas[hpKey].journeyFail;
                    heroInfos[i].power += LoadGameData.Instance.herosDatas[powerKey].journeyFail;
                    heroInfos[i].stress += LoadGameData.Instance.herosDatas[stressKey].journeyFail;
                    heroInfos[i].exp += LoadGameData.Instance.herosDatas[expKey].journeyFail;
                    heroStateList[i].SetState(heroInfos[i]);
                }
                break;
        }
    }

    //void Update()
    //{

    //}

    public void SlideAll(bool _isSlide)
    {
        for (int i = 0; i < heroStateList.Length; i++)
        {
            heroStateList[i].isSlide = _isSlide;
            heroStateList[i].Slide();
        }
    }
}

public class HeroInfo
{
    public float hp;
    public float power;
    public float stress;
    public float exp;

    public HeroInfo(float _hp, float _power, float _stress, float _exp)
    {
        this.hp = _hp;
        this.power = _power;
        this.stress = _stress;
        this.exp = _exp;
    }
}