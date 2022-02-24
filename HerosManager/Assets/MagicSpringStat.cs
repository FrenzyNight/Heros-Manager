using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpringStat : MonoBehaviour
{
    private MagicSpringManager SM;
    private bool isActive;

    private float activeTime;
    // Start is called before the first frame update
    void Start()
    {
        SM = GameObject.Find("MagicSpringManager").GetComponent<MagicSpringManager>();
        isActive= false;
        activeTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            ActiveSpring();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            DeActiveSpring();
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
    }

    void ActiveSpring()
    {
        isActive = true;
        gameObject.GetComponent<Animator>().SetBool("isActive", true);
        //gameObject.GetComponent<Animator>().Play("SpringActiveAni", -1, 0f);
    }

    void DeActiveSpring()
    {
        gameObject.GetComponent<Animator>().SetBool("isActive", false);
    }
}
