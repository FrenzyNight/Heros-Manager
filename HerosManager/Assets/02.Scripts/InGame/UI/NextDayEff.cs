using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextDayEff : MonoBehaviour
{
    public Text NextText;
    string nextStr = "다음날...";

    void OnEnable()
    {
        StartCoroutine("NextDayEffCo");
    }

    void OnDisable()
    {
        StopCoroutine("NextDayEffCo");
    }

    IEnumerator NextDayEffCo()
    {
        int idx = 0;
        while (true)
        {
            NextText.text = nextStr.Substring(0, idx);

            yield return new WaitForSeconds(0.2f);

            idx++;
            if (idx > nextStr.Length)
            {
                idx = 0;
            }
        }
    }
}
