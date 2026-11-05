using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MiniGameSetMgr
{
    public GameObject Wall, Road;
    public float blockSize;
    public int mazesize;
    public float mazeCharSpeed;
    public float realMazeCharSpeed;
    private const float MazeStandardSpeed = 300f;
    private int lastTreasureFrame = -1;

    void Start()
    {
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();
        lastTreasureFrame = -1;
        mazeCharSpeed = GameData("game2maze").Velocity;
        realMazeCharSpeed = MazeStandardSpeed * mazeCharSpeed;
        MakeMaze();
    }

    private void MakeMaze()
    {
        if (Wall == null || Road == null || Wall.GetComponent<RectTransform>() == null ||
            Road.GetComponent<RectTransform>() == null)
            throw new System.InvalidOperationException("Maze prefabs require assigned RectTransforms.");
        if (float.IsNaN(blockSize) || float.IsInfinity(blockSize) || blockSize <= 0f)
            throw new System.InvalidOperationException("Maze block size must be finite and positive.");

        bool[,] roads = MiniGameRules.GenerateMaze(mazesize, count => Random.Range(0, count));
        Transform container = SpawnContainer("MazeStructure");
        StopGame();
        for (int row = 0; row < mazesize; row++)
        {
            for (int column = 0; column < mazesize; column++)
            {
                GameObject block = Spawn(roads[row, column] ? Road : Wall, transform.position,
                    Quaternion.identity, container);
                block.GetComponent<RectTransform>().anchoredPosition = new Vector2(column * blockSize, -row * blockSize);
            }
        }
    }

    public void ResetMaze()
    {
        foreach (var character in SpawnContainer("MazeStructure").parent.GetComponentsInChildren<MazeCharMove>(true))
            character.ResetPosition();
        MakeMaze();
    }

    public void CollectTreasure()
    {
        if (!IsPrepared || !isActiveAndEnabled || lastTreasureFrame == Time.frameCount) return;
        lastTreasureFrame = Time.frameCount;
        var reward = GameData("game2maze");
        if (reward.GetAmount1 > 0) AddItem(reward.GetItemID1, (int)reward.GetAmount1);
        if (reward.GetAmount2 > 0) AddItem(reward.GetItemID2, (int)reward.GetAmount2);
        if (reward.GetJemAmount > 0) MiniGameMgr.Instance.AddTempGems((int)reward.GetJemAmount);
        GamepadInput.Pulse(0.18f, 0.3f, 0.12f);
        ResetMaze();
    }

    void Update()
    {
        if (GamepadUiController.MiniGameInputAllowed && (Input.GetKeyDown(KeyCode.Space) || GamepadInput.Pressed(GamepadButton.Y)))
            ResetMaze();
    }

    public override void StartGame() { }
}
