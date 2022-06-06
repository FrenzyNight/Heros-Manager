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

    void Start()
    {
        CloseBtn.onClick.AddListener(ExitBtnEvent);
        ExitBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Title");
        });
        OptionBtn.onClick.AddListener(() =>
        {
            optionMgr.OpenPanel();
        });
        QuitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        TitleTxt.text = LoadGameData.Instance.GetString("Pause_t1");

        CloseBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Pause_t2");
        ExitBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Pause_t3");
        OptionBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t4");
        QuitBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t5");
    }

    public void PauseBtn()
    {
        this.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitBtnEvent()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
