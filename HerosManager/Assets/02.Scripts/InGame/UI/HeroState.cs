using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public Text NameText;
    public Text JobText;
    public TextMeshProUGUI StateText;
    public Text ExpText;
    public Image ExpFill;

    string statStr;

    public Button button;

    RectTransform rectTrans;
    public bool isSlide;
    Vector2 closeSlideVec;
    Vector2 openSlideVec;
    Vector2 slideVec;

    Coroutine slideCo;

    void Start()
    {
        rectTrans = this.GetComponent<RectTransform>();
        isSlide = false;
        closeSlideVec = rectTrans.anchoredPosition;
        openSlideVec = new Vector2(-rectTrans.rect.width / 2, rectTrans.anchoredPosition.y);

        button.onClick.AddListener(() => { Slide(); });
    }

    public void Setup(HeroData _heroData)
    {
        NameText.text = LoadGameData.Instance.GetString(_heroData.HeroStringID);
        //JobText.text = LoadGameData.Instance.GetString(_heroData.HeroStringID);

        stress = LoadGameData.Instance.heroStateDatas[_heroData.StressID].FirstGive;
        power = LoadGameData.Instance.heroStateDatas[_heroData.PowerID].FirstGive;
        hp = LoadGameData.Instance.heroStateDatas[_heroData.HpID].FirstGive;
        exp = LoadGameData.Instance.heroStateDatas[_heroData.ExpID].FirstGive;

        minStress = LoadGameData.Instance.heroStateDatas[_heroData.StressID].Min;
        maxStress = LoadGameData.Instance.heroStateDatas[_heroData.StressID].Max;
        minPower = LoadGameData.Instance.heroStateDatas[_heroData.PowerID].Min;
        maxPower = LoadGameData.Instance.heroStateDatas[_heroData.PowerID].Max;
        minHp = LoadGameData.Instance.heroStateDatas[_heroData.HpID].Min;
        maxHp = LoadGameData.Instance.heroStateDatas[_heroData.HpID].Max;
        minExp = LoadGameData.Instance.heroStateDatas[_heroData.ExpID].Min;
        maxExp = LoadGameData.Instance.heroStateDatas[_heroData.ExpID].Max;

        statStr = LoadGameData.Instance.GetString(_heroData.StressStringID) + ":{0}\n" +
            LoadGameData.Instance.GetString(_heroData.PowerStringID) + ":{1}\n" +
            LoadGameData.Instance.GetString(_heroData.HpStringID) + ":{2}/" + LoadGameData.Instance.heroStateDatas[_heroData.HpID].Max;
        StateText.text = string.Format(statStr, stress, power, hp);

        ExpText.text = LoadGameData.Instance.GetString(_heroData.ExpStringID);

        ExpFill.fillAmount = exp / maxExp;
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


        StateText.text = string.Format(statStr, stress, power, hp);
        ExpFill.fillAmount = exp / maxExp;
    }

    public void Slide()
    {
        if (isSlide)
            slideVec = closeSlideVec;
        else
            slideVec = openSlideVec;

        if (slideCo != null)
            StopCoroutine(slideCo);
        slideCo = StartCoroutine(SlideCo());

        isSlide = !isSlide;
    }

    IEnumerator SlideCo()
    {
        while (Vector2.Distance(rectTrans.anchoredPosition, slideVec) >= 0.1f)
        {
            rectTrans.anchoredPosition = Vector2.Lerp(rectTrans.anchoredPosition, slideVec, Time.deltaTime * 3f);

            yield return null;
        }
    }
}