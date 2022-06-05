using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite3DAnimator : MonoBehaviour
{
    public enum AnimationAxis { Rows, Columns }

    public MeshRenderer meshRenderer;
    public string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    public AnimationAxis axis;
    public float animationSpeed = 5f;
    public int animationIndex = 0;
    public bool isMove = false;

    void Update()
    {
        string clipKey, frameKey;
        if (axis == AnimationAxis.Rows)
        {
            clipKey = rowProperty;
            frameKey = colProperty; 
        }
        else
        {
            clipKey = colProperty;
            frameKey = rowProperty;
        }

        //Animate
        if(isMove)
        {
            int frame = (int)(Time.time * animationSpeed);
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        }
        
    }

    public void SetIdle()
    {
        isMove = false;
        meshRenderer.material.SetFloat(rowProperty, 1);
        meshRenderer.material.SetFloat(colProperty, animationIndex);
    }
}
