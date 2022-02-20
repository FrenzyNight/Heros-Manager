using UnityEngine;

public class FireMgr : CampObjectMgr
{
    public GameObject FireObj;
    public GameObject FoodObj;

    [ContextMenu("OnFire")]
    void OnFire()
    {
        FireObj.SetActive(true);
    }

    [ContextMenu("NonFire")]
    void NonFire()
    {
        FireObj.SetActive(false);
    }

    [ContextMenu("OnCook")]
    void OnCook()
    {
        FoodObj.SetActive(true);
    }

    [ContextMenu("NonCook")]
    void NonCook()
    {
        FoodObj.SetActive(false);
    }
}
