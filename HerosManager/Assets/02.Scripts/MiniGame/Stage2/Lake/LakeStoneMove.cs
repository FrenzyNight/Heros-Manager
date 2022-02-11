using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeStoneMove : MonoBehaviour
{
    private LakeWaterManager WM;
    private RectTransform rt;
    public float direction;
    public int rockNum;

    public float realSpeed;
    // Start is called before the first frame update
    void Start()
    {
        WM = GameObject.Find("LakeWaterManager").GetComponent<LakeWaterManager>();
        rt = gameObject.GetComponent<RectTransform>();
        direction = 1;

        if(rockNum == 1)
        {
            realSpeed = WM.realRock1Speed;
        }
        else if(rockNum == 2)
        {
            realSpeed = WM.realRock2Speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(rt.anchoredPosition.x >= WM.rockMovePoint)
        {
            direction = -1;
        }
        else if(rt.anchoredPosition.x <= -WM.rockMovePoint)
        {
            direction = 1;
        }
    }

    void FixedUpdate()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (realSpeed * direction * Time.deltaTime), rt.anchoredPosition.y);
    }
}
