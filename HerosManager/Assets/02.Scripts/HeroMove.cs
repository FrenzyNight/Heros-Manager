using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeroMove : MonoBehaviour
{
    public float moveTime;
    public float standTime;
    public float speed;
    public float maxX, maxY;
    public float minX, minY;
    public float moveX, moveY;
    public GameObject CharSprite;
    public GameObject parent;
    private Animator ani;

    // Start is called before the first frame update
    void Start()
    {
        ani = CharSprite.GetComponent<Animator>();
        SetAdv();
        //StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        
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
                parent.transform.DORotate(new Vector3(30,180,0), 0.5f);
            }
            else
            {
                parent.transform.DORotate(new Vector3(-30,0,0), 0.5f);
            }

            ani.SetBool("isMove", true);
            transform.DOMove(new Vector3(moveX,gameObject.transform.position.y, moveY), moveTime);
            yield return new WaitForSeconds(moveTime);

            ani.SetBool("isMove", false);
            yield return new WaitForSeconds(standTime);
        }
    }

    public void ComeBackHome()
    {
        /*
        moveX = Random.Range(20, 22);
        moveY = Random.Range(19, 21);

        transform.DOMove(new Vector3(moveX, 0.5f, moveY),2f);
        */
        StartCoroutine(Move());
    }

    public void SetAdv()
    {

        StopCoroutine(Move());
        transform.position = new Vector3(21, 0.5f, 40);
    }
}
