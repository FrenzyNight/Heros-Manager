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

        SetMoveTime();
        SetDirection();
    }

    public override IEnumerator SetIdle() => IdleFor(Mgr.huntMonsterIdleTime);

    public override void SetMoveTime()
    {
        ChooseMoveTime(Mgr.huntMonsterMinMoveTime, Mgr.huntMonsterMaxMoveTime);
    }

    public override void Angry()
    {
        if (AngryEffect != null)
            AngryEffect.SetActive(true);
        realSpeed = realSpeed * Mgr.huntBearAngrySpeed;
        SetDirection();
    }

    public override void Death()
    {
        if (!TryBeginDeath())
            return;
        // 곰 사망
        Mgr.KillBear();
        //Mgr.huntBearRes 만큼 고기 획득
        Debug.Log("Get Meat " + Mgr.huntBearRes.ToString());
        Mgr.bearNum = Mathf.Max(0, Mgr.bearNum - 1);
        Destroy(gameObject);
    }  

    
}
