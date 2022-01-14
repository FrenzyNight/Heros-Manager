using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameMgr : MonoBehaviour
{
    public GameObject Pannel;
    public GameObject ShadowPanel;

    public GameObject Forest;
    public GameObject Grassland;
    public GameObject River;

    public GameObject itemText;


    public int wood = 0;
    public int water = 0;
    public int meat = 0;
    public int hub = 0;
    public int jem = 0;

    GameObject currentGame;

    //etc

    public float firstTime;
    public float textSpeed;
    public float textPosition;

    public void StartMiniGame(string _stageCode)
    {
        Pannel.SetActive(true);
        ShadowPanel.SetActive(true);

        GameObject obj = null;
        switch (_stageCode)
        {
            case "Stage_1_Forest":
                obj = Forest;
                //CaveManager.Setup()
                //....

                InGameMgr.Instance.state = State.Game1;
                obj.SetActive(true);

                obj.transform.Find("Wood/WoodManager").GetComponent<WoodManager>().StartGame();
                obj.transform.Find("Cave/CaveManager").GetComponent<CaveManager>().StartGame();
                
                break;

            case "Stage_1_Grassland":
                obj = Grassland;

                InGameMgr.Instance.state = State.Game2;
                obj.SetActive(true);

                obj.transform.Find("Hub/HubManager").GetComponent<HubManager>().StartGame();
                
                obj.transform.Find("Hunt/HuntManager").GetComponent<HuntManager>().StartGame();
                break;

            case "Stage_1_river":
                obj = River;

                InGameMgr.Instance.state = State.Game3;
                obj.SetActive(true);

                obj.transform.Find("Fish/FishManager").GetComponent<FishManager>().StartGame();
                obj.transform.Find("Water/WaterManager").GetComponent<WaterManager>().StartGame();


                break;
        }

        
        currentGame = obj;

        Gathering.Instance.GatheringPannel.SetActive(false);
    }

    void Update()
    {
        itemText.GetComponent<Text>().text = wood.ToString() + ", " + water.ToString() + ", " + meat.ToString() + ", " + hub.ToString() + ", " + jem.ToString();
    
    }

    public void CloseCurrentGame()
    {
        ItemManager.Instance.AddItem(wood, water, meat, hub, jem);
        ResetItem();
        currentGame.SetActive(false);
        Pannel.SetActive(false);
        ShadowPanel.SetActive(false);
        Gathering.Instance.OpenGatheringPannel();
    }

    void ResetItem()
    {
        wood = 0;
        water = 0;
        meat = 0;
        hub = 0;
        jem = 0;
    }
}
