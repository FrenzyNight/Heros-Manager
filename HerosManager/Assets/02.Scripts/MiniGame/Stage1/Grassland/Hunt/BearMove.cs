  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMove : MiniGameAnimalMove
{
    private HuntManager Mgr;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Mgr = manager.GetComponent<HuntManager>();
        
        animalHP = Mgr.huntBearHP;

        realSpeed = Mgr.standardHuntSpeed * Mgr.huntBearSpeed;

        objectType = 4;
    }

    public override IEnumerator SetIdle()
    {
        isIdle = true;
        ani.SetBool("isIdle", true);
        yield return new WaitForSeconds(Mgr.huntMonsterIdleTime);
        SetDirection();
        SetMoveTime();
        isIdle = false;
        ani.SetBool("isIdle", false);

    }

    public override void SetMoveTime()
    {
        moveCheck = 0;
        moveTime = Random.Range(Mgr.huntMonsterMinMoveTime,Mgr.huntMonsterMaxMoveTime);
    }

    public override void Angry()
    {
        AngryEffect.SetActive(true);
        realSpeed = realSpeed * Mgr.huntBearAngrySpeed;
        SetDirection();
    }

    public override void Death()
    {
        // 곰 사망
        Mgr.KillBear();
        //Mgr.huntBearRes 만큼 고기 획득
        Debug.Log("Get Meat " + Mgr.huntBearRes.ToString());
        Mgr.bearNum -= 1;
        Destroy(gameObject);
    }  

    
}
