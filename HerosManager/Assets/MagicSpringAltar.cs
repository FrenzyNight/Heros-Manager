using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpringAltar : MonoBehaviour
{
    private MagicSpringManager SM;
    public GameObject Fire, Circle;
    public bool isActive;
    private float activeTime;
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
        
    }

    public void Active()
    {
        isActive =true;
        Fire.GetComponent<Animator>().SetBool("isActive", true);
        Circle.GetComponent<Animator>().SetBool("isActive", true);
    }

    void DeActive()
    {
        isActive =false;
        Fire.GetComponent<Animator>().SetBool("isActive", false);
        Circle.GetComponent<Animator>().SetBool("isActive", false);
    }
}
