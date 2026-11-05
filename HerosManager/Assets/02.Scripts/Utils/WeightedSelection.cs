using System;
using System.Collections.Generic;

/// <summary>Select from non-normalized weights with an injected sample for deterministic validation.</summary>
public static class WeightedSelection
{
    public static bool TryPick<T>(IReadOnlyList<T> values, Func<T, float> weight, float unitSample, out T selected)
    {
        selected = default;
        if (values == null || weight == null || float.IsNaN(unitSample) || unitSample < 0f || unitSample > 1f)
            return false;

        var weights = new float[values.Count];
        double total = 0;
        int lastEligible = -1;
        for (int i = 0; i < values.Count; i++)
        {
            float value = weight(values[i]);
            if (float.IsNaN(value) || float.IsInfinity(value) || value <= 0f)
                continue;
            weights[i] = value;
            total += value;
            lastEligible = i;
        }
        if (lastEligible < 0)
            return false;

        double target = total * unitSample;
        double accumulated = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            accumulated += weights[i];
            if (weights[i] > 0f && (target < accumulated || i == lastEligible))
            {
                selected = values[i];
                return true;
            }
        }
        return false;
    }
}
