using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeFishMove : MonoBehaviour
{
    private LakeFishManager FM;
    private RectTransform rt;

    private bool isHooked;


    private int direction;
    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("LakeFishManager").GetComponent<LakeFishManager>();
        rt = gameObject.GetComponent<RectTransform>();
        
        isHooked = false;

        Destroy(gameObject, 10f);
    }

    public void SetDir(int dir)
    {
        if(dir == -1)
        {
            direction = -1;
        }

        else if(dir == 1)
        {
            direction = 1;
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    void FixedUpdate()
    {
        if(!isHooked)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + direction * (FM.realFishSpeed * Time.deltaTime), rt.anchoredPosition.y);
        else if(isHooked)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (FM.realFishLineSpeed * FM.fishLineCatchSpeedVar * Time.deltaTime));
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Hook"))
        {
            isHooked = true;
        }
    }
}
