using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicSpringCommand : MonoBehaviour
{
    private MagicSpringManager SM;
    public GameObject[] keypads;
    public Sprite[] keyImg;
    public int[] keycode = new int[7];

    private int now;
    private bool isStun;
    private bool isActive;
    private int altarNum;
    // Start is called before the first frame update
    void Start()
    {
        SM = GameObject.Find("MagicSpringManager").GetComponent<MagicSpringManager>();
        now = 0;
        isStun = false;
        isActive = false;
        //SetUp();     
    }

 
    public void SetUp(int num)
    {
        altarNum = num;
        isActive = true;
        for(int i=0;i<7;i++)
        {
            keycode[i] = Random.Range(0,8);
            keypads[i].GetComponent<Image>().sprite = keyImg[keycode[i]];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isStun && isActive)
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                InputCommand(0);
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                InputCommand(1);
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                InputCommand(2);
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                InputCommand(3);
            }

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                InputCommand(4);
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                InputCommand(5);
            }
             if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                InputCommand(6);
            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                InputCommand(7);
            }
            
        }
    }

    void InputCommand(int key)
    {
        if(keycode[now] == key)
        {
            Debug.Log("Collect");
            keypads[now].GetComponent<Image>().color = new Color(255,0,0);
            now++;
        }
        else
        {
            Debug.Log("Wrong");
            StartCoroutine(Stun());
            for(int  i=0;i<=now;i++)
            {
                keypads[i].GetComponent<Image>().color = new Color(255,255,255);
            }
            now = 0;
        }

        if(now == 8)
        {
            //complite
            
            isActive = false;
            SM.AltarActive(altarNum);
            gameObject.SetActive(false);
            Debug.Log("Complite");
        }
    }

    IEnumerator Stun()
    {
        isStun = true;
        yield return new WaitForSeconds(SM.msKeyPadStun);

        isStun = false;
    }
}
