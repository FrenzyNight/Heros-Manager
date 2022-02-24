using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCtrl : MonoBehaviour
{
    public MovePosMgr movePosMgr;

    NavMeshAgent navAgent;

    Animator anim;

    private void Awake()
    {
        navAgent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponentInChildren<Animator>();
    }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        navAgent.speed = 5f;
        navAgent.angularSpeed = 720f;
        navAgent.acceleration = 20f;
    }

    void Update()
    {
        switch (InGameMgr.Instance.state)
        {
            case State.Camp:
                Move_MouseClick();
                break;
            case State.Game1:
                break;
            case State.Game2:
                break;
            case State.Game3:
                break;
        }
    }

    void Move_MouseClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.tag == "Land")
                {
                    MoveTo(hits[i].point);
                    break;
                }
            }
        }
    }

    void MoveTo(Vector3 _pos)
    {
        StopCoroutine("OnMoveCo");

        movePosMgr.MovePosEff(_pos);
        anim.SetBool("isMove", true);
        navAgent.SetDestination(_pos);

        StartCoroutine("OnMoveCo");
    }

    IEnumerator OnMoveCo()
    {
        while (true)
        {
            if (Vector3.Distance(navAgent.destination, this.transform.position) < 0.1f)
            {
                this.transform.position = navAgent.destination;

                navAgent.ResetPath();

                anim.SetBool("isMove", false);

                movePosMgr.gameObject.SetActive(false);

                break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CampObject")
        {
            other.GetComponentInParent<CampObjectMgr>().OpenUI();
            print(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CampObject")
        {
            other.GetComponentInParent<CampObjectMgr>().CloseUI();
            print(2);
        }
    }
}
