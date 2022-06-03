using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public CutSceneManager cutSceneMgr;

    void Start()
    {
        LoadGameData.Instance.LoadCSVDatas();
        cutSceneMgr.LoadCutscene("open_Lay");
        cutSceneMgr.NextCutscene();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            bool isLast = cutSceneMgr.NextCutscene();
            if (isLast)
            {
                SceneManager.LoadScene("InGame");
            }
        }
    }
}
