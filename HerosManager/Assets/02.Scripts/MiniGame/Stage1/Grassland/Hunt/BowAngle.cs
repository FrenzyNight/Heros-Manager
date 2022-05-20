using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAngle : MiniGameCharMgr
{
    AudioSource audioSource;
    private HuntManager Mgr;
    private Animator ani;

    // Start is called before the first frame update
    void Start()
    {
        Mgr = manager.GetComponent<HuntManager>();
        ani = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        target = transform.position;

        isCheck = false;

        objectType = 5;
        moveType = 3;
        //moveType = LoadGameData.Instance.miniGameDatas["game2Lay"].value1;
    }

    public override void Action1()
    {
        StartCoroutine(ShootArrow());
    }

    IEnumerator ShootArrow()
    {
        isCheck = true;
        audioSource.Play();
        Instantiate(Mgr.ArrowPrefab, target, Quaternion.AngleAxis(angle, Vector3.forward), Mgr.mother.transform);

        ani.Play("Bow_ani", -1, 0f);
        yield return new WaitForSeconds(Mgr.huntArrowCoolTime);
       
        isCheck = false;

        yield return null;

    }



    public override IEnumerator Stun()
    {
        yield return new WaitForSeconds(Mgr.huntArrowCoolTime);
        Mgr.isCheck = false;
        yield return null;
    }
}
