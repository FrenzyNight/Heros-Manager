using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public CutSceneManager cutSceneMgr;

    public Button SkipBtn;

    private bool isLeaving;

    void Start()
    {
        Time.timeScale = 1f;
        LoadGameData.Instance.LoadTitleDatas();
        SkipBtn.onClick.AddListener(EnterGame);

        cutSceneMgr.LoadCutscene("open_Lay");
        cutSceneMgr.NextCutscene();
    }

    void Update()
    {
        if (!isLeaving && Input.anyKeyDown)
        {
            bool isLast = cutSceneMgr.NextCutscene();
            if (isLast)
            {
                EnterGame();
            }
        }
    }

    private void EnterGame()
    {
        if (isLeaving) return;
        isLeaving = true;
        SceneManager.LoadScene("InGame");
    }

    private void OnDestroy()
    {
        if (SkipBtn != null) SkipBtn.onClick.RemoveListener(EnterGame);
    }
}
