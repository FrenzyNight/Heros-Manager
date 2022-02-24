using UnityEngine;

public class FenceMgr : CampObjectMgr
{
    public GameObject fenceLv1;
    public GameObject fenceLv2;
    public GameObject fenceLv3;

    [ContextMenu("FenceLv1")]
    void FenceLv1()
    {
        fenceLv1.SetActive(true);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(false);
    }

    [ContextMenu("FenceLv2")]
    void FenceLv2()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(true);
        fenceLv3.SetActive(false);
    }

    [ContextMenu("FenceLv3")]
    void FenceLv3()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(true);
    }
}
