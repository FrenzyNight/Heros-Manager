using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public CutSceneManager cutSceneMgr;

    public Button SkipBtn;

    void Start()
    {
        SkipBtn.onClick.AddListener(() =>
        {
            //SceneManager.LoadScene("MapTest");
            SceneManager.LoadScene("InGame");
        });

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
                //SceneManager.LoadScene("MapTest");
                SceneManager.LoadScene("InGame");
            }
        }
    }
}
