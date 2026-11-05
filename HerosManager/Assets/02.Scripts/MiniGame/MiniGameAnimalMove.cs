using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameAnimalMove : MiniGameObjectMgr
{
    AudioSource audioSource;
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
    private bool dead;
    private bool angered;
    protected virtual string ArrowTag => "MiniGameObj3";
    protected virtual string ObstacleTag => "MiniGameObj1";
    protected virtual string AnimalTag => "MiniGameObj2";
    protected virtual bool MoveToForeground => true;

    protected virtual void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        if (rt == null)
            throw new MissingComponentException($"{name} requires a RectTransform.");
        if (MoveToForeground)
            rt.SetAsLastSibling();
        ani = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        isIdle = false;
        dead = false;
        angered = false;
    }

    public override void MoveAction()
    {
        if(!isIdle && !dead)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (x * Time.deltaTime) , rt.anchoredPosition.y + (y * Time.deltaTime)); 
            moveCheck += Time.deltaTime;
        }
    }

    public override void UpdateAction()
    {
        if(!isIdle && !dead && moveCheck >= moveTime)
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

    public virtual IEnumerator SetIdle() { yield return IdleFor(0.02f); }

    protected IEnumerator IdleFor(float duration)
    {
        isIdle = true;
        if (ani != null) ani.SetBool("isIdle", true);
        yield return new WaitForSeconds(MiniGameRules.PositiveDelay(duration));
        SetDirection();
        SetMoveTime();
        isIdle = false;
        if (ani != null) ani.SetBool("isIdle", false);
    }

    protected void ChooseMoveTime(float minimum, float maximum)
    {
        minimum = MiniGameRules.PositiveDelay(minimum);
        maximum = Mathf.Max(minimum, MiniGameRules.PositiveDelay(maximum));
        moveCheck = 0f;
        moveTime = Random.Range(minimum, maximum);
    }

    protected bool TryBeginDeath()
    {
        if (dead)
            return false;
        dead = true;
        isIdle = true;
        StopAllCoroutines();
        return true;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        //block or player or monster
        if(!dead && (coll.gameObject.CompareTag(ObstacleTag) || coll.gameObject.CompareTag("Player") || coll.gameObject.CompareTag(AnimalTag)))
        {
            SetDirection();
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(!dead && coll.gameObject.activeInHierarchy && coll.gameObject.CompareTag(ArrowTag))
        {
            if (audioSource != null) audioSource.Play();
            coll.gameObject.SetActive(false);
            Destroy(coll.gameObject);
            
            
            
            animalHP -= 1;

            if(animalHP <= 0)
            {
                Death();
            }
            else if(animalHP <= 1 && objectType == 4 && !angered)
            {
                angered = true;
                Angry();
            }
        }
    }
}
