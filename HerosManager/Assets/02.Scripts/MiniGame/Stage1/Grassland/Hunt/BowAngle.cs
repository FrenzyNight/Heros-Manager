using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAngle : MiniGameCharMgr
{
    private HuntManager Mgr;
    public GameObject ArrowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Mgr = manager.GetComponent<HuntManager>();
        target = transform.position;

        isCheck = false;

        moveType = 3;
        //moveType = LoadGameData.Instance.miniGameDatas["game2Lay"].value1;
    }

    public override void Action1()
    {
        isCheck = true;
        Instantiate(ArrowPrefab, target, Quaternion.AngleAxis(angle, Vector3.forward), Mgr.mother.transform);

        gameObject.GetComponent<Animator>().Play("Bow_ani", -1, 0f);

        StartCoroutine(Stun());
    }



    public override IEnumerator Stun()
    {
        yield return new WaitForSeconds(Mgr.huntArrowCoolTime);
        isCheck = false;
        yield return null;
    }
}
