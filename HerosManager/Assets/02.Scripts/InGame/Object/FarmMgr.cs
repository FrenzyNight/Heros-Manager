using UnityEngine;

public class FarmMgr : CampObjectMgr
{
    public GameObject BigPlant;
    public GameObject SmallPlant;

    [ContextMenu("OnFarm")]
    void OnWash()
    {
        BigPlant.SetActive(true);
        SmallPlant.SetActive(true);
    }

    [ContextMenu("NonFarm")]
    void NonWash()
    {
        BigPlant.SetActive(false);
        SmallPlant.SetActive(false);
    }

}
