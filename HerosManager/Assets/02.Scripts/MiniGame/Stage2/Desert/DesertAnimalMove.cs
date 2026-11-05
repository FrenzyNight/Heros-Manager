using System.Collections;
using UnityEngine;

public abstract class DesertAnimalMove : MiniGameAnimalMove
{
    protected Hunt2Manager Hunt { get; private set; }
    protected override string ArrowTag => "Arrow";
    protected override string ObstacleTag => "Block";
    protected override string AnimalTag => "Monster";
    protected override bool MoveToForeground => false;

    protected override void Start()
    {
        base.Start();
        Hunt = MiniGameReferences.Manager<Hunt2Manager>(this, manager);
        ConfigureAnimal();
        SetMoveTime();
        SetDirection();
    }

    protected abstract void ConfigureAnimal();

    public override IEnumerator SetIdle() => IdleFor(Hunt.huntMonsterIdleTime);

    public override void SetMoveTime()
    {
        ChooseMoveTime(Hunt.huntMonsterMinMoveTime, Hunt.huntMonsterMaxMoveTime);
    }

    protected void ApplyAnger(float multiplier)
    {
        if (AngryEffect != null)
            AngryEffect.SetActive(true);
        realSpeed *= multiplier;
        SetDirection();
    }
}
