using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaselMove : DesertAnimalMove
{
    protected override void ConfigureAnimal()
    {
        animalHP = Hunt.huntWeaselHP;
        realSpeed = Hunt.realWeaselSpeed;
        objectType = 3;
    }

    public override void Death()
    {
        if (!TryBeginDeath())
            return;
        Hunt.KillWeasel();
        Hunt.weaselNum = Mathf.Max(0, Hunt.weaselNum - 1);
        Destroy(gameObject);
    }
}
