using UnityEngine;

public class FarmMgr : CampObjectMgr
{
    public GameObject BigPlant;
    public GameObject MediumPlant;
    public GameObject SmallPlant;

    [ContextMenu("OnFarm")]
    public void OnFarm(int state)
    {
        switch(state)
        {
            case 1:
                BigPlant.SetActive(false);
                MediumPlant.SetActive(false);
                SmallPlant.SetActive(true);
                break;
            case 2:
                BigPlant.SetActive(false);
                MediumPlant.SetActive(true);
                SmallPlant.SetActive(false);
                break;
            case 3:
                BigPlant.SetActive(true);
                MediumPlant.SetActive(false);
                SmallPlant.SetActive(false);
                break;
        }

        BigPlant.SetActive(true);
        MediumPlant.SetActive(true);
        SmallPlant.SetActive(true);
    }

    [ContextMenu("NonFarm")]
    public void NonFarm()
    {
        BigPlant.SetActive(false);
        MediumPlant.SetActive(false);
        SmallPlant.SetActive(false);
    }

}
