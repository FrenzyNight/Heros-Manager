using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameSetMgr : MonoBehaviour
{
    [Header("MiniGame Standard")]
    public GameObject mother;
    public float widthScale;
    public float heightScale;
    public int rnd;
    public bool isCheck;
    public Vector2 BlockSpawnPoint;
    [HideInInspector]public Vector2 RealSpawnPoint;

    public string miniGameID;
    public string item1ID;
    public string item2ID;

    public List<GameObject> spawnObjects = new List<GameObject>();

    protected bool IsPrepared { get; private set; }

    public virtual void SetUp()
    {
        StopGame();
        IsPrepared = true;
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        RealSpawnPoint = new Vector2(BlockSpawnPoint.x * widthScale + transform.position.x , BlockSpawnPoint.y * heightScale + transform.position.y);
    }

    public virtual void StartGame()
    {
        StopGame();
    }

    protected GameObject TrackSpawn(GameObject spawned)
    {
        spawnObjects.RemoveAll(item => item == null);
        spawnObjects.Add(spawned);
        return spawned;
    }

    /// <summary>Stop producers before removing their objects so reopening starts a fresh session.</summary>
    public virtual void StopGame()
    {
        StopAllCoroutines();
        CancelInvoke();
        foreach (GameObject spawned in spawnObjects)
        {
            if (spawned == null)
                continue;
            spawned.SetActive(false);
            Destroy(spawned);
        }
        spawnObjects.Clear();
    }

    protected virtual void OnDisable()
    {
        StopGame();
        IsPrepared = false;
    }

    public virtual void AddItem(string itemID, int amount)
    {
        MiniGameMgr.Instance.AddTempItem(itemID, amount);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(RealSpawnPoint ,5f);
    }
}
