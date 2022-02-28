using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpringAltar : MonoBehaviour
{
    private MagicSpringManager SM;
    public GameObject Fire, Circle;
    public bool isActive;
    private float activeTime;
    private float rndTime;
    public int AltarNum;
    // Start is called before the first frame update
    void Start()
    {
        SM = GameObject.Find("MagicSpringManager").GetComponent<MagicSpringManager>();
        isActive = false;
        activeTime = 0;
    
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            activeTime += Time.deltaTime;
        }

        if(isActive && activeTime > rndTime)
        {
            DeActive();
        }
    }

    public void Active()
    {
        isActive =true;
        Fire.GetComponent<Animator>().SetBool("isActive", true);
        Circle.GetComponent<Animator>().SetBool("isActive", true);

        activeTime = 0;
        rndTime = Random.Range(SM.msAlterMinTime, SM.msAlterMaxTime);
    }

    void DeActive()
    {
        isActive =false;
        Fire.GetComponent<Animator>().SetBool("isActive", false);
        Circle.GetComponent<Animator>().SetBool("isActive", false);

        SM.AltarDeActive(AltarNum);
    }
}
