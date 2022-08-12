using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCameraMove : MonoBehaviour
{
    public GameObject charObj;

    private Transform charTransform;

    private Vector3 offset;
    private bool isFallowStart = false;
    void Awake()
    {
        charTransform = charObj.transform;
        //offset = transform.position - charTransform.position;
    }

    private void Update()
    {
        if (!isFallowStart)
        {
            if (charTransform.position.z < transform.position.z)
            {
                isFallowStart = true;
                offset = transform.position - charTransform.position;
            }
        }
    }

    private void LateUpdate()
    {
        if (isFallowStart)
        {
            transform.position = charTransform.position + offset;
        }
        
    }
}
