using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public Text TitleTxt;

    public Button CloseBtn;
    public Button ExitBtn;
    public Button OptionBtn;
    public Button QuitBtn;

    public OptionManager optionMgr;
    float previousTimeScale = 1f;
    bool paused;
    public bool IsOpen => paused && gameObject.activeInHierarchy;

    void Start()
    {
        CloseBtn.onClick.AddListener(ExitBtnEvent);
        ExitBtn.onClick.AddListener(ReturnToTitle);
        OptionBtn.onClick.AddListener(OpenOptions);
        QuitBtn.onClick.AddListener(Quit);

        TitleTxt.text = LoadGameData.Instance.GetString("Pause_t1");

        CloseBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Pause_t2");
        ExitBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Pause_t3");
        OptionBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t4");
        QuitBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t5");
    }

    public void PauseBtn()
    {
        var game = InGameMgr.Instance;
        if (paused || (!game.IsComplete && (Clock.Instance.isProduct ||
            (game.state == State.None && !AdventureManager.Instance.AdvPanel.activeInHierarchy))))
            return;
        paused = true;
        previousTimeScale = Time.timeScale;
        this.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitBtnEvent()
    {
        if (!paused)
            return;
        paused = false;
        this.gameObject.SetActive(false);
        Time.timeScale = previousTimeScale;
    }

    void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    void OpenOptions() { optionMgr.OpenPanel(); }
    void Quit() { Application.Quit(); }

    void OnDestroy()
    {
        if (CloseBtn != null) CloseBtn.onClick.RemoveListener(ExitBtnEvent);
        if (ExitBtn != null) ExitBtn.onClick.RemoveListener(ReturnToTitle);
        if (OptionBtn != null) OptionBtn.onClick.RemoveListener(OpenOptions);
        if (QuitBtn != null) QuitBtn.onClick.RemoveListener(Quit);
    }
}
