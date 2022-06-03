using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CharProduct : MonoBehaviour
{
    AudioSource audioSource;
    public GameObject PadePanel;
    public GameObject[] HerosSprite;

    public float target;
    public float waitTime;
    public float moveTime;
    public Ease ease;
    public AudioClip birdSound1, birdSound2;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Invoke("HerosMove", waitTime);

        PadePanel.GetComponent<Image>().DOFade(0,1.5f);
        Invoke("SoundStart",1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SoundStart()
    {
        audioSource.Play();
    }

    void HerosMove()
    {
        Debug.Log("Move");
        gameObject.transform.DOMove(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, target), moveTime).SetEase(ease);
        Invoke("HeroStop", moveTime);
        Invoke("FadeIn", moveTime + 0.5f);
    }

    void HeroStop()
    {
        for(int i=0; i<5; i++)
        {
            HerosSprite[i].GetComponent<Animator>().SetBool("isMove", false);
        }
    }

    void FadeIn()
    {
        PadePanel.GetComponent<Image>().DOFade(1,1.5f);
        Invoke("ChangeScene", 1.5f);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("InGame");
    }
}
