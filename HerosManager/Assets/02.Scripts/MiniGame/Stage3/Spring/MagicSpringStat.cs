using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpringStat : MonoBehaviour
{
    private MagicSpringManager SM;
    private bool isActive;

    private float activeTime;
    private float graceCount;
    // Start is called before the first frame update
    void Start()
    {
        SM = GameObject.Find("MagicSpringManager").GetComponent<MagicSpringManager>();
        isActive= false;
        activeTime = 0;
        graceCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(SM.isAltar[0] &&SM.isAltar[1] &&SM.isAltar[2] &&SM.isAltar[3])
        {
            ActiveSpring();
        }

        if(isActive)
        {
            activeTime+= Time.deltaTime;
        }

        if(activeTime>= SM.msSpringWaterTime)
        {
            activeTime = 0;
            SM.GetWater();
        }

        if(graceCount >= SM.msSpringGraceTime)
        {
            DeActiveSpring();
        }

        if(isActive && (!SM.isAltar[0] || !SM.isAltar[1] || !SM.isAltar[2] || !SM.isAltar[3]))
        {
            //grace
            graceCount += Time.deltaTime;
        }
    }

    void ActiveSpring()
    {
        graceCount = 0;
        if(!isActive)
        {
            isActive = true;
            gameObject.GetComponent<Animator>().SetBool("isActive", true);
        }
        
        //gameObject.GetComponent<Animator>().Play("SpringActiveAni", -1, 0f);
    }

    void DeActiveSpring()
    {
        activeTime = 0;
        isActive = false;
        gameObject.GetComponent<Animator>().SetBool("isActive", false);
    }
}
