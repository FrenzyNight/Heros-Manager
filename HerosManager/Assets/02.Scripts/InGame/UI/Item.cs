using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int num;
    public Text NumText;
    public Text EffNum;

    Coroutine runningCo;

    public void Setup(int _num)
    {
        num = Mathf.Max(0, _num);
        Refresh();
    }

    public void AddNum(int _addNum)
    {
        if (_addNum == 0)
            return;

        int previous = num;
        num = InventoryRules.ApplyDelta(num, _addNum);
        Refresh(num - previous);
    }

    public void Refresh(int delta = 0)
    {
        if (NumText != null)
            NumText.text = num.ToString();
        if (delta == 0 || EffNum == null || !isActiveAndEnabled)
            return;

        if (runningCo != null)
            StopCoroutine(runningCo);
        runningCo = StartCoroutine(AddNumEffCo(delta));
    }

    IEnumerator AddNumEffCo(int _addNum)
    {
        RectTransform rectTrans = EffNum.GetComponent<RectTransform>();

        string str = "";
        if (_addNum >= 0)
            str = "+";

        EffNum.text = str + _addNum;
        rectTrans.anchoredPosition = Vector2.zero;
        EffNum.color = Color.white;
        EffNum.gameObject.SetActive(true);

        Color tempColor = EffNum.color;
        Vector2 tempVec = rectTrans.anchoredPosition;
        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / 1.5f;
            EffNum.color = tempColor;

            tempVec.y += Time.deltaTime * 20f;
            rectTrans.anchoredPosition = tempVec;

            yield return null;
        }

        EffNum.gameObject.SetActive(false);
        runningCo = null;
    }

    void OnDisable()
    {
        if (runningCo != null)
            StopCoroutine(runningCo);
        runningCo = null;
        if (EffNum != null)
            EffNum.gameObject.SetActive(false);
    }
}
