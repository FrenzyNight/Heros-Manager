using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CutSceneManager : MonoBehaviour
{
    public GameObject BackPanel;
    public Image PrevImg, Img;
    public TextMeshProUGUI StringText;
    public GameObject Wating;
    public AudioSource audioSource, typingAS;
    public AudioClip typingSound;

    private IReadOnlyList<CutsceneData> lines = System.Array.Empty<CutsceneData>();
    private int nextIndex;
    private bool imageTransition;
    private float volume;
    private Coroutine typing, sound, waiting;

    public void LoadCutscene(string code)
    {
        StopEffects();
        LoadGameData.Instance.LoadTitleDatas();
        lines = LoadGameData.Instance.Catalog.GetCutscenes(code);
        nextIndex = 0;
        volume = SaveDataManager.Instance.saveOptionData.EffVolume;
        typingAS.clip = typingSound;
        typingAS.volume = volume;
        StringText.text = string.Empty;
        StringText.maxVisibleCharacters = int.MaxValue;
        PrevImg.gameObject.SetActive(false);
    }

    public bool NextCutscene()
    {
        if (imageTransition) return false;
        StopWaiting();
        if (typing != null)
        {
            StopCoroutine(typing);
            typing = null;
            StringText.maxVisibleCharacters = int.MaxValue;
            typingAS.Stop();
            waiting = StartCoroutine(Waiting());
            return false;
        }
        if (nextIndex >= lines.Count)
        {
            StopEffects();
            return true;
        }

        var line = lines[nextIndex++];
        ApplyImage(line);
        StringText.text = line.StringID == "-1" ? string.Empty : LoadGameData.Instance.GetString(line.StringID);
        StringText.maxVisibleCharacters = line.StringType == 1 ? 0 : int.MaxValue;
        if (line.StringType == 1) typing = StartCoroutine(TypeLine(line.StringDelay));
        else waiting = StartCoroutine(Waiting());
        ApplySound(line);
        return false;
    }

    private void ApplyImage(CutsceneData line)
    {
        if (line.ImageType < 1 || line.ImageType > 3) return;
        Img.DOKill();
        imageTransition = true;
        if (line.ImageType == 2)
        {
            PrevImg.sprite = Img.sprite;
            PrevImg.gameObject.SetActive(true);
        }
        if (line.ImageType != 3)
            Img.sprite = Resources.Load<Sprite>("CutsceneImage/" + line.ImageResourceID);
        Img.color = new Color(1, 1, 1, line.ImageType == 3 ? 1 : 0);
        Img.DOFade(line.ImageType == 3 ? 0 : 1, Mathf.Max(0, line.value2))
            .SetDelay(Mathf.Max(0, line.ImageDelay)).SetEase(Ease.Linear).SetUpdate(true).SetId(this)
            .OnComplete(() =>
            {
                imageTransition = false;
                PrevImg.gameObject.SetActive(false);
            });
    }

    private IEnumerator TypeLine(float delay)
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0, delay));
        StringText.ForceMeshUpdate();
        int count = StringText.textInfo.characterCount;
        var interval = new WaitForSecondsRealtime(0.1f);
        for (int visible = 1; visible <= count; visible++)
        {
            StringText.maxVisibleCharacters = visible;
            typingAS.Play();
            yield return interval;
        }
        typing = null;
        waiting = StartCoroutine(Waiting());
    }

    private IEnumerator Waiting()
    {
        var interval = new WaitForSecondsRealtime(0.5f);
        while (true)
        {
            Wating.SetActive(!Wating.activeSelf);
            yield return interval;
        }
    }

    private void StopWaiting()
    {
        if (waiting != null) StopCoroutine(waiting);
        waiting = null;
        Wating.SetActive(false);
    }

    private void ApplySound(CutsceneData line)
    {
        if (sound != null) StopCoroutine(sound);
        sound = null;
        audioSource.DOKill();
        if (line.SoundType == 1 || line.SoundType == 2)
            sound = StartCoroutine(PlaySound(line));
        else if (line.SoundType == 3)
            audioSource.DOFade(0, Mathf.Max(0, line.value3)).SetDelay(Mathf.Max(0, line.SoundDelay))
                .SetUpdate(true).SetId(this).OnComplete(audioSource.Stop);
        else if (line.SoundType == 4) audioSource.Stop();
    }

    private IEnumerator PlaySound(CutsceneData line)
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0, line.SoundDelay));
        audioSource.clip = Resources.Load<AudioClip>("CutsceneSound/" + line.SoundResourceID);
        audioSource.loop = line.SoundType == 2;
        audioSource.volume = volume;
        audioSource.Play();
        sound = null;
    }

    private void StopEffects()
    {
        StopAllCoroutines();
        typing = sound = waiting = null;
        DOTween.Kill(this);
        imageTransition = false;
        if (Wating != null) Wating.SetActive(false);
        if (audioSource != null) audioSource.Stop();
        if (typingAS != null) typingAS.Stop();
    }

    private void OnDisable() => StopEffects();
}
