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

    void Start()
    {
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();
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
        MakeMaze();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ResetMaze();
    }

    public override void StartGame() { }
}
