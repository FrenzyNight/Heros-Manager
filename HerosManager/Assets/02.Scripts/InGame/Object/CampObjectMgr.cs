using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampObjectMgr : MonoBehaviour
{
    public GameObject ObjectUI;

    void Start()
    {
        ObjectUI.transform.LookAt(ObjectUI.transform.position
            + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.down);
    }

    [ContextMenu("OpenUI")]
    public virtual void OpenUI()
    {
        ObjectUI.SetActive(true);
    }

    [ContextMenu("CloseUI")]
    public virtual void CloseUI()
    {
        ObjectUI.SetActive(false);
    }
}
