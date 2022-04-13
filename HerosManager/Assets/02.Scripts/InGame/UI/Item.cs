using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int num;
    public Text NumText;
    public Text EffNum;

    Coroutine runningCo;

    public void Setup(ItemData _itemData)
    {
        num = _itemData.FirstGive;
        NumText.text = num.ToString();
    }

    public void AddNum(int _addNum)
    {
        if (_addNum == 0)
            return;

        num += _addNum;
        NumText.text = num.ToString();

        if (runningCo != null)
            StopCoroutine(runningCo);
        runningCo = StartCoroutine(AddNumEffCo(_addNum));
    }

    IEnumerator AddNumEffCo(int _addNum)
    {
        RectTransform rectTrans = EffNum.GetComponent<RectTransform>();

        string str = "";
        if (_addNum < 0)
            str = "-";
        else
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
    }
}
