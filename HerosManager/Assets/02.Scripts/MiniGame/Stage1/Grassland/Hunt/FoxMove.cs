using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxMove : MiniGameAnimalMove
{
    private HuntManager Mgr;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Mgr = manager.GetComponent<HuntManager>();
        
        animalHP = Mgr.huntFoxHP;
        
        realSpeed = Mgr.realFoxSpeed;
        
        objectType = 3;

        SetMoveTime();
        SetDirection();
    }

    public override IEnumerator SetIdle() => IdleFor(Mgr.huntMonsterIdleTime);
    
    public override void SetMoveTime()
    {
        ChooseMoveTime(Mgr.huntMonsterMinMoveTime, Mgr.huntMonsterMaxMoveTime);
    }

    public override void Death()
    {
        if (!TryBeginDeath())
            return;
        // 여우 사망
        Mgr.KillFox();
        Debug.Log("Get Meat " + Mgr.huntFoxRes.ToString());
        Mgr.foxNum = Mathf.Max(0, Mgr.foxNum - 1);
        Destroy(gameObject);

    }   
}
