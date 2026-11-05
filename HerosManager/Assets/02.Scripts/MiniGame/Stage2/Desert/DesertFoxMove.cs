using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertFoxMove : DesertAnimalMove
{
    protected override void ConfigureAnimal()
    {
        animalHP = Hunt.huntFoxHP;
        realSpeed = Hunt.realFoxSpeed;
        objectType = 4;
    }

    public override void Angry() => ApplyAnger(Hunt.huntFoxAngrySpeed);

    public override void Death()
    {
        if (!TryBeginDeath())
            return;
        Hunt.KillFox();
        Hunt.foxNum = Mathf.Max(0, Hunt.foxNum - 1);
        Destroy(gameObject);
    }
}
