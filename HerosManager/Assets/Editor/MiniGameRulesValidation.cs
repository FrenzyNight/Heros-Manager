using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MiniGameRulesValidation
{
    public static void Run() => Validate();

    [MenuItem("Tools/Heros Manager/Validate Minigame Rules")]
    public static void Validate()
    {
        ValidateRewards();
        ValidateWeights();
        ValidateMovement();
        ValidateMaze();
        ValidateHierarchy();
        ValidateSpawning();
        Debug.Log("MINIGAME RULES VALIDATION PASSED");
    }

    private static void ValidateRewards()
    {
        var ledger = new MiniGameRewardLedger();
        Require(!ledger.Add("Item_Wood", 10), "Closed rewards cannot accumulate.");
        Require(ledger.Begin() && !ledger.Begin(), "Beginning twice must preserve the active session.");
        Require(ledger.Add("Item_Wood", int.MaxValue), "Known rewards are accepted.");
        ledger.Add("Item_Wood", 1);
        Require(ledger[0] == int.MaxValue, "Rewards saturate rather than overflow.");
        ledger.Add("Item_Water", -1);
        Require(ledger[1] == 0 && !ledger.Add("Missing", 10), "Negative and unknown rewards are bounded.");
        Require(ledger.TryComplete(out int[] rewards) && rewards[0] == int.MaxValue, "Session pays its recorded reward.");
        Require(!ledger.TryComplete(out _) && !ledger.Add("Item_Wood", 1), "A paid session cannot be paid again.");
        ledger.Begin();
        Require(ledger[0] == 0, "A new session begins empty.");
        ledger.Add("Item_Meat", 10);
        ledger.Cancel();
        Require(!ledger.TryComplete(out _), "Cancelled sessions do not pay rewards.");
        Require(MiniGameRules.PositiveDelay(float.NaN) > 0f && MiniGameRules.PositiveDelay(float.PositiveInfinity) > 0f,
            "Invalid timer values cannot create unbounded producers.");
        Require(MiniGameRules.RequiredSpawnCount(2, 3.5f) == 2, "Spawn batches preserve fractional upper-bound behavior.");
        Throws<ArgumentException>(() => MiniGameRules.RequiredSpawnCount(0, float.PositiveInfinity));
        Throws<ArgumentOutOfRangeException>(() => MiniGameRules.RequiredSpawnCount(0, 1001));
        Require(!MiniGameRules.PercentHit(1, 0) && MiniGameRules.PercentHit(100, 100), "Probability endpoints.");
        Require(!MiniGameRules.PercentHit(1, float.NaN), "Invalid probability never succeeds.");
    }

    private static void ValidateWeights()
    {
        float[] weights = { 0, 1, 3, 0 };
        Require(WeightedSelection.TryPick(weights, value => value, 0f, out float first) && first == 1f,
            "Zero probability entries must not be selected at the lower boundary.");
        Require(WeightedSelection.TryPick(weights, value => value, 0.25f, out float boundary) && boundary == 3f,
            "Cumulative boundary belongs to the following positive interval.");
        Require(WeightedSelection.TryPick(weights, value => value, 1f, out float last) && last == 3f,
            "Upper boundary selects the final positive weight.");
        Require(!WeightedSelection.TryPick(new[] { 0f, -1f, float.NaN, float.PositiveInfinity }, value => value, 0.5f, out _),
            "There is no result when all weights are unusable.");
        Require(!WeightedSelection.TryPick(weights, value => value, float.NaN, out _), "Invalid samples are rejected.");
        Require(WeightedSelection.TryPick(new[] { float.MaxValue, float.MaxValue }, value => value, 0.75f, out _),
            "Weight totals use sufficient range.");
        int selectedFirst = 0;
        for (int i = 0; i < 1000; i++)
        {
            WeightedSelection.TryPick(weights, value => value, (i + 0.5f) / 1000f, out float chosen);
            if (chosen == 1f) selectedFirst++;
        }
        Require(selectedFirst == 250, "Non-normalized 1:3 weights partition the sample domain correctly.");
    }

    private static void ValidateMovement()
    {
        Require(MiniGameMovement.Direction(true, false, true, false) == Vector2.zero, "Opposing input cancels.");
        Require(MiniGameMovement.Direction(true, true, false, true) == Vector2.up, "Three held keys preserve the remaining axis.");
        Require(Mathf.Approximately(MiniGameMovement.Direction(true, false, false, true).magnitude, 1f),
            "Diagonal movement preserves cardinal speed.");
    }

    private static void ValidateMaze()
    {
        foreach (int size in new[] { 3, 5, 11, 31, 101 })
        {
            for (int seed = 0; seed < 4; seed++)
            {
                var random = new System.Random(seed);
                bool[,] roads = MiniGameRules.GenerateMaze(size, random.Next);
                int roadCount = 0;
                int edges = 0;
                for (int row = 0; row < size; row++)
                    for (int column = 0; column < size; column++)
                    {
                        if (!roads[row, column]) continue;
                        roadCount++;
                        if (row + 1 < size && roads[row + 1, column]) edges++;
                        if (column + 1 < size && roads[row, column + 1]) edges++;
                    }
                int cellsPerSide = (size - 1) / 2;
                Require(roadCount == 2 * cellsPerSide * cellsPerSide + 1, "Maze opens every logical cell and exactly two entrances.");
                Require(edges == roadCount - 1, "Maze does not contain cycles.");
                var visited = new HashSet<int> { size + 1 };
                var queue = new Queue<int>();
                queue.Enqueue(size + 1);
                while (queue.Count > 0)
                {
                    int cell = queue.Dequeue();
                    int row = cell / size;
                    int column = cell % size;
                    foreach (Vector2Int delta in new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left })
                    {
                        int nextRow = row + delta.y;
                        int nextColumn = column + delta.x;
                        int next = nextRow * size + nextColumn;
                        if (nextRow >= 0 && nextRow < size && nextColumn >= 0 && nextColumn < size &&
                            roads[nextRow, nextColumn] && visited.Add(next))
                            queue.Enqueue(next);
                    }
                }
                Require(visited.Count == roadCount && roads[size - 1, 1] && roads[0, size - 2], "Both entrances and every road are connected.");
            }
        }
        Throws<ArgumentOutOfRangeException>(() => MiniGameRules.GenerateMaze(2, count => 0));
        Throws<ArgumentOutOfRangeException>(() => MiniGameRules.GenerateMaze(102, count => 0));
        Throws<InvalidOperationException>(() => MiniGameRules.GenerateMaze(5, count => count));
    }

    private static void ValidateHierarchy()
    {
        var panel = new GameObject("Minigame panel");
        var outside = new GameObject("Outside panel");
        try
        {
            var stage = new GameObject("Stage");
            stage.transform.SetParent(panel.transform);
            var firstArea = new GameObject("First area");
            firstArea.transform.SetParent(stage.transform);
            var secondArea = new GameObject("Second area");
            secondArea.transform.SetParent(stage.transform);
            var first = new GameObject("First game");
            first.transform.SetParent(firstArea.transform);
            var second = new GameObject("Second game");
            second.transform.SetParent(secondArea.transform);
            first.SetActive(false);
            second.SetActive(false);
            stage.SetActive(false);
            firstArea.SetActive(false);
            var changed = new List<GameObject>();
            MiniGameReferences.ActivateWithin(first, panel.transform, changed);
            MiniGameReferences.ActivateWithin(second, panel.transform, changed);
            Require(first.activeInHierarchy && second.activeInHierarchy, "Both games activate through inactive stage ancestors.");
            Require(changed.Count == 2 && !changed.Contains(secondArea), "Only inactive ancestors are owned, and shared paths are recorded once.");
            Throws<InvalidOperationException>(() => MiniGameReferences.ActivateWithin(outside, panel.transform, changed));
            first.SetActive(false);
            second.SetActive(false);
            for (int index = changed.Count - 1; index >= 0; index--)
                changed[index].SetActive(false);
            Require(!stage.activeSelf && !firstArea.activeSelf && secondArea.activeSelf,
                "Closing restores only ancestors changed by the minigame session.");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(outside);
            UnityEngine.Object.DestroyImmediate(panel);
        }
    }

    private static void ValidateSpawning()
    {
        var root = new GameObject("Minigame validation root");
        var source = new GameObject("Spawn source");
        try
        {
            var controllerObject = new GameObject("Controller");
            controllerObject.transform.SetParent(root.transform);
            var controller = controllerObject.AddComponent<MiniGameSetMgr>();
            var original = source.AddComponent<MiniGameObjectMgr>();
            var spawned = controller.Spawn(source, Vector3.zero, Quaternion.identity);
            Require(original.manager == null && spawned.GetComponent<MiniGameObjectMgr>().manager == controllerObject,
                "Spawning binds the instance without modifying the original prefab.");
            Require(controller.mother == null && spawned.transform.parent == root.transform,
                "Unassigned legacy containers fall back to the controller's game root.");
            controller.TrackSpawn(spawned);
            Require(controller.spawnObjects.Count == 1, "Registering a spawn twice cannot duplicate ownership.");
            Require(MiniGameReferences.Manager<MiniGameSetMgr>(spawned.transform) == controller,
                "Participants resolve the controller inside their hierarchy.");
            controller.StopGame();
            controller.StopGame();
            Require(controller.spawnObjects.Count == 0 && spawned == null, "Stopping twice removes only owned instances.");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(source);
            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}.");
    }
}
