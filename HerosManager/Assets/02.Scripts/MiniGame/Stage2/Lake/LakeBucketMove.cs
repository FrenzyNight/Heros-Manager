using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LakeBucketMove : MonoBehaviour
{
    public Sprite lv0Img,lv1Img,lv2Img;
    private LakeWaterManager WM;
    private RectTransform rt;
    public float direction;

    private int lv;
    public float moveVar;
    public float endP;
    public float topP,botP;
    public float lv1Line, lv2Line;
    private Vector2 initialPosition;
    private int previousAim;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        initialPosition = rt.anchoredPosition;
    }

    void OnEnable()
    {
        rt.anchoredPosition = initialPosition;
        previousAim = 0;
        lv = 0;
        direction = 1;
        ImgChange();
    }

    // Start is called before the first frame update
    void Start()
    {
        WM = MiniGameReferences.Manager<LakeWaterManager>(this);
        rt = gameObject.GetComponent<RectTransform>();
        lv = 0;
        direction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GamepadUiController.MiniGameInputAllowed) return;
        int aim = GamepadInput.Aim.x < -0.4f ? -1 : GamepadInput.Aim.x > 0.4f ? 1 : 0;
        if(Input.GetKeyDown(KeyCode.LeftArrow) || (aim == -1 && previousAim != -1))
        {
            LeftMove();
        }

        if(Input.GetKeyDown(KeyCode.RightArrow) || (aim == 1 && previousAim != 1))
        {
            RightMove();
        }

        previousAim = aim;
        if(Input.GetKeyDown(KeyCode.UpArrow) || GamepadInput.Pressed(GamepadButton.RightShoulder))
        {
            direction = -1;
        }

        if(rt.anchoredPosition.y <= lv1Line && lv == 0)
        {
            lv = 1;
            ImgChange();
        }

        if(rt.anchoredPosition.y <= lv2Line && lv == 1)
        {
            lv = 2;
            ImgChange();
        }

        if(rt.anchoredPosition.y <= botP && lv == 2)
        {
            direction = -1;
        }

        if(rt.anchoredPosition.y >= topP)
        {
            if(lv != 0)
                WM.GetWater(lv);
            ResetBucket();
        }

    }

    void FixedUpdate()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - (WM.realBucketSpeed * direction * Time.deltaTime));
    }

    void ImgChange()
    {
        if(lv == 0)
        {
            gameObject.GetComponent<Image>().sprite = lv0Img;
        }
        else if(lv == 1)
        {
            gameObject.GetComponent<Image>().sprite = lv1Img;
        }
        else if(lv == 2)
        {
            gameObject.GetComponent<Image>().sprite = lv2Img;
        }

    }

    void ResetBucket()
    {
        lv = 0;
        direction = 1;
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, topP);
        ImgChange();
    }
    void LeftMove()
    {
        if(rt.anchoredPosition.x > -endP)
        {
            rt.anchoredPosition = new Vector2(Mathf.Max(-endP, rt.anchoredPosition.x - moveVar), rt.anchoredPosition.y);
        }
    }
    void RightMove()
    {
        if(rt.anchoredPosition.x < endP)
        {
            rt.anchoredPosition = new Vector2(Mathf.Min(endP, rt.anchoredPosition.x + moveVar), rt.anchoredPosition.y);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Block"))
        {
            ResetBucket();
        }
    }
}
