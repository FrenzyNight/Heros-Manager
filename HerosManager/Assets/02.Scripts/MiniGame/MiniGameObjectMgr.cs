using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameObjectMgr : MonoBehaviour
{
    public GameObject manager;

    public int objectType;

    // Update is called once per frame
    void Update()
    {
        if(objectType == 2 || objectType == 3 || objectType == 4 || objectType == 5) // if object need UpdateAction
        {
            UpdateAction();
        }
    }

    void FixedUpdate()
    {
        if(objectType == 1 || objectType == 3 || objectType == 4 || objectType == 5) // if object is moving object
        {
            MoveAction();
        }
    }

    public virtual void MoveAction() {}

    public virtual void UpdateAction() {}
}
