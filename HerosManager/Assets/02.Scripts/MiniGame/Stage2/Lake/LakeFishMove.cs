using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeFishMove : MonoBehaviour
{
    private LakeFishManager FM;
    private RectTransform rt;

    private bool isHooked;
    private float lifetime;


    private int direction;
    // Start is called before the first frame update
    void Start()
    {
        FM = MiniGameReferences.Manager<LakeFishManager>(this);
        rt = gameObject.GetComponent<RectTransform>();
        
        isHooked = false;

        lifetime = 10f;
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
        {
            float speed = CompareTag("GoldFish") ? FM.realGoldFishSpeed : FM.realFishSpeed;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + direction * speed * Time.deltaTime, rt.anchoredPosition.y);
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f) Destroy(gameObject);
        }
        else if(isHooked)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (FM.realFishLineSpeed * FM.fishLineCatchSpeedVar * Time.deltaTime));
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Hook") && !isHooked &&
            coll.TryGetComponent<LakeHookMove>(out var hook) && hook.TryCatchFish())
        {
            isHooked = true;
        }
    }
}
