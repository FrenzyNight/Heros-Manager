using UnityEngine;

public class LaundryMgr : CampObjectMgr
{
    public GameObject LaundryObject;
    public GameObject hangingLaundryObject;

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

    [ContextMenu("OnHanging")]
    public void OnHanging()
    {
        hangingLaundryObject.SetActive(true);
    }

    [ContextMenu(("NonHanging"))]
    public void NonHanging()
    {
        hangingLaundryObject.SetActive(false);
    }
}
