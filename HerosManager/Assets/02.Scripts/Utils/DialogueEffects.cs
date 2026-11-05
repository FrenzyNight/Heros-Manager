using System.Collections;
using UnityEngine;

public static class DialogueEffects
{
    public static void PlayTypingSound(AudioSource source, ref float nextSoundTime)
    {
        if (source == null || source.clip == null || source.isPlaying || Time.unscaledTime < nextSoundTime) return;
        source.loop = false;
        source.Play();
        nextSoundTime = Time.unscaledTime + 0.075f;
    }

    public static IEnumerator PulseWaiting(GameObject indicator)
    {
        var group = indicator.GetComponent<CanvasGroup>();
        if (group == null) group = indicator.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        group.interactable = false;
        float elapsed = 0;
        indicator.SetActive(true);
        while (true)
        {
            group.alpha = 0.55f + 0.45f * (0.5f + 0.5f * Mathf.Cos(elapsed * Mathf.PI * 2f));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
