using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampObjectMgr : MonoBehaviour
{
    public GameObject ObjectUI;
    //public GameObject mainCamera;
    [HideInInspector]
    public Camera cameraToLookAt;

    void Start()
    {
        /*
        cameraToLookAt = mainCamera.GetComponent<Camera>();
        
        ObjectUI.transform.LookAt(ObjectUI.transform.position
            + cameraToLookAt.transform.rotation * Vector3.back,
        cameraToLookAt.transform.rotation * Vector3.down);
        */
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
