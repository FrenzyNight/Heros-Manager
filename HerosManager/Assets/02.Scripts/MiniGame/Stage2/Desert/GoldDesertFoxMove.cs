using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDesertFoxMove : DesertAnimalMove
{
    protected override void ConfigureAnimal()
    {
        animalHP = Hunt.huntGoldFoxHP;
        realSpeed = Hunt.realGoldFoxSpeed;
        objectType = 4;
    }

    public override void Angry() => ApplyAnger(Hunt.huntGoldFoxAngrySpeed);

    public override void Death()
    {
        if (!TryBeginDeath())
            return;
        Hunt.KillGoldFox();
        Hunt.goldFoxNum = Mathf.Max(0, Hunt.goldFoxNum - 1);
        Destroy(gameObject);
    }
}
