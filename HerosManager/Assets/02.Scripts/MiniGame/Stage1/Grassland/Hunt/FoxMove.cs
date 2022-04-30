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
        moveTime = Random.Range(Mgr.huntMonsterMinMoveTime, Mgr.huntMonsterMaxMoveTime);
    }

    public override void Death()
    {
        // 여우 사망
        Mgr.KillFox();
        Debug.Log("Get Meat " + Mgr.huntFoxRes.ToString());
        Mgr.foxNum -= 1;
        Destroy(gameObject);
    }   
}
