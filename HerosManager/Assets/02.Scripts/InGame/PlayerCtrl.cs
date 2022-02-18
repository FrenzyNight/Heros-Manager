using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    Vector3 movePosVec;
    float moveSpeed;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        movePosVec = this.transform.position;
        moveSpeed = 2f;
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
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Map")
                    movePosVec = hit.point;
            }
        }

        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosVec, moveSpeed * Time.deltaTime);
    }
}
