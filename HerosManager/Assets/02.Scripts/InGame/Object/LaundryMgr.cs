using UnityEngine;

public class LaundryMgr : CampObjectMgr
{
    public GameObject LaundryObject;

    [ContextMenu("OnWash")]
    public void OnWash()
    {
        LaundryObject.SetActive(true);
    }

    [ContextMenu("NonWash")]
    public void NonWash()
    {
        LaundryObject.SetActive(false);
    }
}
