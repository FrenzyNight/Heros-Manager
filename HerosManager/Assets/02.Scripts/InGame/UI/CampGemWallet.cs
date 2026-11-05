using UnityEngine;

/// <summary>Scene-owned currency balance, restored from the daily checkpoint.</summary>
[DefaultExecutionOrder(-200)]
public class CampGemWallet : Singleton<CampGemWallet>
{
    private int amount;

    public override void Init()
    {
        amount = Mathf.Max(0, SaveDataManager.Instance.saveGameData?.gems ?? 0);
    }

    public int GetAmount() => amount;

    public bool TrySpend(int cost)
    {
        if (cost < 0 || cost > amount) return false;
        amount -= cost;
        return true;
    }

    public void Add(int reward)
    {
        if (reward > 0) amount = MiniGameRules.AddReward(amount, reward);
    }
}
