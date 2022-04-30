using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameAnimalMove : MiniGameObjectMgr
{
    public RectTransform rt;
    public Animator ani;
    public int direction;
    public float x, y;
    public bool isIdle;
    public float moveTime;
    public float moveCheck;
    public float animalHP;

    public GameObject AngryEffect;
    public float realSpeed;

    protected virtual void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        rt.SetAsLastSibling();
        ani = GetComponent<Animator>();
        isIdle = false;
    }

    public override void MoveAction()
    {
        if(!isIdle)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (x * Time.deltaTime) , rt.anchoredPosition.y + (y * Time.deltaTime)); 
            moveCheck += Time.deltaTime;
        }
    }

    public override void UpdateAction()
    {
        if(!isIdle && moveCheck >= moveTime)
        {
            StartCoroutine(SetIdle());
        }
    }
 
    public virtual void Angry() {}
    public virtual void Death() {}

    public virtual void SetMoveTime() {}
    public void SetDirection() 
    {
        direction = Random.Range(0,360);
        
        x = realSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = realSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);

        if(x >= 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    public virtual IEnumerator SetIdle() {yield return null;}

    void OnCollisionEnter2D(Collision2D coll)
    {
        //block or player or monster
        if(coll.gameObject.CompareTag("MiniGameObj1") || coll.gameObject.CompareTag("Player") || coll.gameObject.CompareTag("MiniGameObj2"))
        {
            SetDirection();
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "MiniGameObj3") //arrow
        {
            Destroy(coll.gameObject);
            
            animalHP -= 1;

            if(animalHP <= 0)
            {
                Death();
            }
            else if(animalHP <= 1 && objectType == 4)
            {
                Angry();
            }
        }
    }
}
