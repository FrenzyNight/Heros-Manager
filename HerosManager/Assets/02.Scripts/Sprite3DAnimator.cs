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
    public int animationIndex;
    public bool isMove;

    private MaterialPropertyBlock properties;
    private int rowId, columnId;

    private void Awake()
    {
        properties = new MaterialPropertyBlock();
        rowId = Shader.PropertyToID(rowProperty);
        columnId = Shader.PropertyToID(colProperty);
    }

    private void Update()
    {
        if (!isMove || meshRenderer == null) return;
        int frame = (int)(Time.time * animationSpeed);
        Apply(axis == AnimationAxis.Rows ? animationIndex : frame,
            axis == AnimationAxis.Rows ? frame : animationIndex);
    }

    public void SetIdle()
    {
        isMove = false;
        Apply(1, animationIndex);
    }

    private void Apply(float row, float column)
    {
        if (meshRenderer == null) return;
        if (properties == null) Awake();
        meshRenderer.GetPropertyBlock(properties);
        properties.SetFloat(rowId, row);
        properties.SetFloat(columnId, column);
        meshRenderer.SetPropertyBlock(properties);
    }
}
