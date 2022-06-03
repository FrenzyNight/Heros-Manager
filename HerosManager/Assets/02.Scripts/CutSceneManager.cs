using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CutSceneManager : MonoBehaviour
{
    public GameObject BackPanel;
    public Image PrevImg;
    public Image Img;
    public TextMeshProUGUI StringText;
    public GameObject Wating;
    public AudioSource audioSource;

    List<CutsceneData> cutSceneList = new List<CutsceneData>();
    int idx;
    bool isSkip;
    float vol;

    IEnumerator TypingCo;
    IEnumerator SoundCo;

    public void LoadCutscene(string _code)
    {
        cutSceneList.Clear();
        idx = 0;
        isSkip = true;
        //vol = SaveDataManager.Instance.saveData.EffVolume;
        vol = 1;
        foreach (var data in LoadGameData.Instance.cutsceneDatas)
        {
            if (data.Value.OpenID == _code)
            {
                cutSceneList.Add(data.Value);
            }
        }
    }

    public bool NextCutscene()
    {
        //string Eff 끝나고 마지막이면
        if (TypingCo == null && idx == cutSceneList.Count)
            return true;

        Wating.SetActive(false);
        StopCoroutine("WatingEffCo");

        if (!isSkip)
            return false;

        if (TypingCo != null)
        {
            StopCoroutine(TypingCo);
            TypingCo = null;
            StringText.text = LoadGameData.Instance.GetString(cutSceneList[idx].StringID);
            idx++;

            StartCoroutine("WatingEffCo");

            return false;
        }

        switch (cutSceneList[idx].ImageType)
        {
            case 1: //fade in
                ImageEff1(cutSceneList[idx].ImageResourceID, cutSceneList[idx].ImageDelay, cutSceneList[idx].value2);
                break;
            case 2: //이전 이미지 그대로 다음 fade in
                ImageEff2(cutSceneList[idx].ImageResourceID, cutSceneList[idx].ImageDelay, cutSceneList[idx].value2);
                break;
            case 3: //fade out
                ImageEff3(cutSceneList[idx].ImageResourceID, cutSceneList[idx].ImageDelay, cutSceneList[idx].value2);
                break;
        }

        switch (cutSceneList[idx].StringType)
        {
            case 1: //타이핑
                TypingCo = TypingEff1Co(cutSceneList[idx].StringID, cutSceneList[idx].StringDelay, cutSceneList[idx].value1);
                StartCoroutine(TypingCo);
                break;
        }

        if (SoundCo != null)
        {
            StopCoroutine(SoundCo);
        }
        audioSource.DOKill();

        switch (cutSceneList[idx].SoundType)
        {
            case 1: //1회 재생
                SoundCo = SoundEff1Co(cutSceneList[idx].SoundResourceID, cutSceneList[idx].StringDelay, cutSceneList[idx].value3);
                StartCoroutine(SoundCo);
                break;
            case 2: //반복 재생
                SoundCo = SoundEff2Co(cutSceneList[idx].SoundResourceID, cutSceneList[idx].StringDelay, cutSceneList[idx].value3);
                StartCoroutine(SoundCo);
                break;
            case 3: //fade out
                SoundEff3(cutSceneList[idx].SoundResourceID, cutSceneList[idx].StringDelay, cutSceneList[idx].value3);
                break;
            case 4: //정지
                audioSource.Stop();
                break;
        }

        return false;
    }

    void ImageEff1(string _imgCode, float _delay, float _value)
    {
        isSkip = false;

        Img.sprite = Resources.Load<Sprite>("CutsceneImage/" + _imgCode);
        Img.color = new Color(255, 255, 255, 0) / 255;

        Img.DOFade(1, _value).SetEase(Ease.Linear).SetDelay(_delay).SetUpdate(true).OnComplete(() =>
        {
            isSkip = true;
        });
    }

    void ImageEff2(string _imgCode, float _delay, float _value)
    {
        isSkip = false;

        PrevImg.gameObject.SetActive(true);
        PrevImg.sprite = Img.sprite;
        Img.sprite = Resources.Load<Sprite>("CutsceneImage/" + _imgCode);
        Img.color = new Color(255, 255, 255, 0) / 255;

        Img.DOFade(1, _value).SetEase(Ease.Linear).SetDelay(_delay).SetUpdate(true).OnComplete(() =>
        {
            PrevImg.gameObject.SetActive(false);
            isSkip = true;
        });
    }

    void ImageEff3(string _imgCode, float _delay, float _value)
    {
        isSkip = false;

        Img.color = Color.white;

        Img.DOFade(0, _value).SetEase(Ease.Linear).SetDelay(_delay).SetUpdate(true).OnComplete(() =>
        {
            isSkip = true;
        });
    }

    IEnumerator TypingEff1Co(string _strCode, float _delay, float _value)
    {
        string str = LoadGameData.Instance.GetString(_strCode);

        yield return new WaitForSecondsRealtime(_delay);

        for (int i = 0; i < str.Length; i++)
        {
            StringText.text = str.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(0.1f);
        }

        idx++;
        TypingCo = null;

        StartCoroutine("WatingEffCo");
    }

    IEnumerator WatingEffCo()
    {
        while (true)
        {
            Wating.SetActive(true);

            yield return new WaitForSecondsRealtime(0.5f);

            Wating.SetActive(false);

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    IEnumerator SoundEff1Co(string _strCode, float _delay, float _value)
    {
        AudioClip clip = Resources.Load<AudioClip>("CutsceneSound/" + _strCode);

        yield return new WaitForSecondsRealtime(_delay);

        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.volume = vol;
        audioSource.Play();
    }

    IEnumerator SoundEff2Co(string _strCode, float _delay, float _value)
    {
        AudioClip clip = Resources.Load<AudioClip>("CutsceneSound/" + _strCode);

        yield return new WaitForSecondsRealtime(_delay);

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = vol;
        audioSource.Play();
    }

    void SoundEff3(string _strCode, float _delay, float _value)
    {
        audioSource.volume = vol;
        audioSource.DOFade(0, cutSceneList[idx].value3).SetEase(Ease.Linear)
                    .SetDelay(cutSceneList[idx].StringDelay).SetUpdate(true).OnComplete(() =>
                    {
                        audioSource.Stop();
                    });
    }
}
