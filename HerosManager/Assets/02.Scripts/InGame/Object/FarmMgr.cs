using UnityEngine;

public class FarmMgr : CampObjectMgr
{
    public GameObject BigPlant;
    public GameObject MediumPlant;
    public GameObject SmallPlant;

    [ContextMenu("OnFarm")]
    void OnFarm()
    {
        BigPlant.SetActive(true);
        MediumPlant.SetActive(true);
        SmallPlant.SetActive(true);
    }

    [ContextMenu("NonFarm")]
    void NonFarm()
    {
        BigPlant.SetActive(false);
        MediumPlant.SetActive(false);
        SmallPlant.SetActive(false);
    }

}
