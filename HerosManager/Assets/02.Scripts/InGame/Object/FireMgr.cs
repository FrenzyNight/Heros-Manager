using UnityEngine;

public class FireMgr : CampObjectMgr
{
    public GameObject FireObj;
    public GameObject FoodObj;

    [ContextMenu("OnFire")]
    public void OnFire()
    {
        FireObj.SetActive(true);
    }

    [ContextMenu("NonFire")]
    public void NonFire()
    {
        FireObj.SetActive(false);
    }

    [ContextMenu("OnCook")]
    public void OnCook()
    {
        FoodObj.SetActive(true);
    }

    [ContextMenu("NonCook")]
    public void NonCook()
    {
        FoodObj.SetActive(false);
    }
}
