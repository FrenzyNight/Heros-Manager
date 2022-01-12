using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameMgr : MonoBehaviour
{
    public GameObject Pannel;

    public GameObject Forest;
    public GameObject Grassland;
    public GameObject River;

    GameObject currentGame;

    public void StartMiniGame(string _stageCode)
    {
        Pannel.SetActive(true);

        GameObject obj = null;
        switch (_stageCode)
        {
            case "Stage_1_Forest":
                obj = Forest;
                //CaveManager.Setup()
                //....

                InGameMgr.Instance.state = State.Game1;
                break;

            case "Stage_1_Grassland":
                obj = Grassland;

                InGameMgr.Instance.state = State.Game2;
                break;

            case "Stage_1_river":
                obj = River;

                InGameMgr.Instance.state = State.Game3;
                break;
        }

        obj.SetActive(true);
        currentGame = obj;

        Gathering.Instance.GatheringPannel.SetActive(false);
    }

    public void CloseCurrentGame()
    {
        currentGame.SetActive(false);
        Pannel.SetActive(false);
        Gathering.Instance.OpenGatheringPannel();
    }
}
