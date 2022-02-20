using UnityEngine;

public class FixMgr : CampObjectMgr
{
    public GameObject WeaponObj;

    [ContextMenu("OnFix")]
    void OnFire()
    {
        WeaponObj.SetActive(true);
    }

    [ContextMenu("NonFix")]
    void NonFire()
    {
        WeaponObj.SetActive(false);
    }
}
