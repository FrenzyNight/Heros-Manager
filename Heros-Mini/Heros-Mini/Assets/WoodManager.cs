using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManager : MonoBehaviour
{
    public GameObject Axe;
    public Vector2 AxePoint1, AxePoint2, AxePoint3, AxePoint4;


    void MoveAxe(int dir)
    {
        if(dir == 1)
        {
            Axe.transform.position = AxePoint1;
        }
        else if(dir == 2)
        {
            Axe.transform.position = AxePoint2;
        }
        else if(dir == 3)
        {
            Axe.transform.position = AxePoint3;
        }
        else if(dir == 4)
        {
            Axe.transform.position = AxePoint4;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            MoveAxe(1);
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            MoveAxe(2);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            MoveAxe(3);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            MoveAxe(4);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(AxePoint1, 0.1f);
        Gizmos.DrawSphere(AxePoint2, 0.1f);
        Gizmos.DrawSphere(AxePoint3, 0.1f);
        Gizmos.DrawSphere(AxePoint4, 0.1f);
        
    }
}
