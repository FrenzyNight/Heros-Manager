using System;
using System.Collections.Generic;

/// <summary>Deterministic rules shared by controllers and editor regression checks.</summary>
public static class MiniGameRules
{
    public static float PositiveDelay(float seconds, float minimum = 0.02f)
    {
        return float.IsNaN(seconds) || float.IsInfinity(seconds) || seconds < minimum ? minimum : seconds;
    }

    public static int AddReward(int current, int amount)
    {
        return (int)Math.Max(0L, Math.Min(int.MaxValue, (long)current + amount));
    }

    public static int RequiredSpawnCount(float current, float maximum)
    {
        if (float.IsNaN(current) || float.IsInfinity(current) || float.IsNaN(maximum) || float.IsInfinity(maximum))
            throw new ArgumentException("Spawn counts must be finite.");
        double count = Math.Ceiling(Math.Max(0d, maximum - Math.Max(0d, current)));
        if (count > 1000)
            throw new ArgumentOutOfRangeException(nameof(maximum), "A single spawn batch cannot exceed 1000 objects.");
        return (int)count;
    }

    public static bool PercentHit(int roll, float percentage)
    {
        if (roll < 1 || roll > 100)
            throw new ArgumentOutOfRangeException(nameof(roll));
        return !float.IsNaN(percentage) && roll <= Math.Max(0f, Math.Min(100f, percentage));
    }

    public static int HorizontalInput(bool left, bool right) => (right ? 1 : 0) - (left ? 1 : 0);
    public static int VerticalInput(bool up, bool down) => (up ? 1 : 0) - (down ? 1 : 0);

    /// <summary>Generate a bounded perfect maze. True is a road; entrances match the legacy layout.</summary>
    public static bool[,] GenerateMaze(int size, Func<int, int> chooseIndex)
    {
        if (size < 3 || size > 101 || size % 2 == 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Maze size must be odd and between 3 and 101.");
        if (chooseIndex == null)
            throw new ArgumentNullException(nameof(chooseIndex));

        var roads = new bool[size, size];
        var visited = new bool[size, size];
        var stack = new Stack<int>();
        int[] rowOffsets = { -2, 0, 2, 0 };
        int[] columnOffsets = { 0, 2, 0, -2 };
        var choices = new int[4];
        stack.Push(size + 1);
        visited[1, 1] = roads[1, 1] = true;

        while (stack.Count > 0)
        {
            int cell = stack.Peek();
            int row = cell / size;
            int column = cell % size;
            int count = 0;
            for (int direction = 0; direction < 4; direction++)
            {
                int nextRow = row + rowOffsets[direction];
                int nextColumn = column + columnOffsets[direction];
                if (nextRow > 0 && nextRow < size - 1 && nextColumn > 0 && nextColumn < size - 1 &&
                    !visited[nextRow, nextColumn])
                    choices[count++] = direction;
            }
            if (count == 0)
            {
                stack.Pop();
                continue;
            }
            int selected = chooseIndex(count);
            if (selected < 0 || selected >= count)
                throw new InvalidOperationException("The maze random source returned an invalid index.");
            int chosen = choices[selected];
            int targetRow = row + rowOffsets[chosen];
            int targetColumn = column + columnOffsets[chosen];
            roads[row + rowOffsets[chosen] / 2, column + columnOffsets[chosen] / 2] = true;
            visited[targetRow, targetColumn] = roads[targetRow, targetColumn] = true;
            stack.Push(targetRow * size + targetColumn);
        }
        roads[size - 1, 1] = true;
        roads[0, size - 2] = true;
        return roads;
    }
}
