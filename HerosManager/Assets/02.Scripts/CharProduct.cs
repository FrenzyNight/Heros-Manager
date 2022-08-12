using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CharProduct : MonoBehaviour
{
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //PadePanel.GetComponent<Image>().DOFade(0,1.5f);
        Invoke("SoundStart",1.5f);
    }

    void SoundStart()
    {
        audioSource.Play();
    }


    public void FadeIn()
    {
        Invoke("ChangeScene", 1.5f);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("InGame");
    }
}
