using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MovePosMgr : MonoBehaviour
{
    public void MovePosEff(Vector3 _pos)
    {
        this.gameObject.SetActive(true);
        this.transform.localScale = new Vector3(7, 7, 7);
        _pos.y = 1;
        this.transform.position = _pos;

        this.transform.DOKill();
        this.transform.DOScale(new Vector3(9, 9, 9), 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
}
