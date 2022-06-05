using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeroMove : MonoBehaviour
{
    public GameObject model;
    [HideInInspector]
    public Sprite3DAnimator sprite3D;

    public float minMoveTime, maxMoveTime;
    [HideInInspector]
    public float moveTime;
    public float minStandTime, maxStandTime;
    [HideInInspector]
    public float standTime;
    public float maxX, maxY;
    public float minX, minY;
    [HideInInspector]
    public float moveX, moveY;

    // Start is called before the first frame update
    void Start()
    {
        sprite3D = model.GetComponent<Sprite3DAnimator>();
        SetAdv();
        //StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        //yield return new WaitForSeconds(2f);
        while(true)
        {
            moveX = Random.Range(minX, maxX);
            moveY = Random.Range(minY, maxY);

            if(gameObject.transform.position.x - moveX < 0) // 왼쪽
            {
                sprite3D.animationIndex = 0;
            }
            else
            {
                sprite3D.animationIndex = 1;
            }

            sprite3D.isMove = true;
            moveTime = Random.Range(minMoveTime, maxMoveTime);
            transform.DOMove(new Vector3(moveX,gameObject.transform.position.y, moveY), moveTime).SetId("HeroMoveTween");
            yield return new WaitForSeconds(moveTime);

            
            sprite3D.SetIdle();
            standTime = Random.Range(minStandTime, maxStandTime);
            yield return new WaitForSeconds(standTime);
        }
    }

    Coroutine coroutine;
    public void ComeBackHome()
    {
        coroutine = StartCoroutine(Move());
    }

    public void SetAdv()
    {
        DOTween.Kill("HeroMoveTween");
        transform.position = new Vector3(21, 0.5f, 45);
        
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}
