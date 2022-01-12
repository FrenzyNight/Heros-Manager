using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public void PauseBtn()
    {
        this.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitBtn()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
