using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCtrl : MonoBehaviour
{
    Vector3 movePosVec;

    NavMeshAgent navAgent;

    private void Awake()
    {
        navAgent = this.GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        movePosVec = this.transform.position;

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
                    movePosVec = hits[i].point;
                    break;
                }
            }
        }

        navAgent.SetDestination(movePosVec);
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
