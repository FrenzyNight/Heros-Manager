using UnityEngine;

public class LaundryMgr : CampObjectMgr
{
    public GameObject LaundryObject;

    [ContextMenu("OnWash")]
    void OnWash()
    {
        LaundryObject.SetActive(true);
    }

    [ContextMenu("NonWash")]
    void NonWash()
    {
        LaundryObject.SetActive(false);
    }
}
