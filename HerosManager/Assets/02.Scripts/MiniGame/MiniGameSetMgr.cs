using System.Collections;
using System.Collections.Generic;
using System;
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
    private bool stopping;
    private readonly Dictionary<string, Transform> containers = new Dictionary<string, Transform>();

    public virtual void SetUp()
    {
        StopGame();
        IsPrepared = true;
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        RealSpawnPoint = ScreenPoint(BlockSpawnPoint);
    }

    public virtual void StartGame()
    {
        StopGame();
    }

    protected Vector2 ScreenPoint(Vector2 point)
    {
        return new Vector2(point.x * widthScale + transform.position.x, point.y * heightScale + transform.position.y);
    }

    protected MiniGameData GameData(string id)
    {
        if (!LoadGameData.Instance.miniGameDatas.TryGetValue(id, out var data))
            throw new InvalidOperationException($"{GetType().Name} requires mini-game table row '{id}'.");
        return data;
    }

    protected Transform SpawnContainer(string legacyName)
    {
        if (!containers.TryGetValue(legacyName, out var container) || container == null)
            containers[legacyName] = container = MiniGameReferences.Container(this, legacyName, mother);
        return container;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null)
            throw new MissingReferenceException($"{GetType().Name} has an unassigned spawn prefab.");
        if (parent == null)
            parent = mother != null ? mother.transform : transform.parent;
        if (parent == null)
            throw new MissingReferenceException($"{GetType().Name} requires a spawn container.");
        GameObject spawned = Instantiate(prefab, position, rotation, parent);
        foreach (MiniGameObjectMgr participant in spawned.GetComponentsInChildren<MiniGameObjectMgr>(true))
            participant.manager = gameObject;
        return TrackSpawn(spawned);
    }

    public GameObject TrackSpawn(GameObject spawned)
    {
        if (spawned == null)
            throw new ArgumentNullException(nameof(spawned));
        if (spawnObjects == null)
            spawnObjects = new List<GameObject>();
        spawnObjects.RemoveAll(item => item == null);
        if (!spawnObjects.Contains(spawned))
            spawnObjects.Add(spawned);
        return spawned;
    }

    /// <summary>Stop producers before removing their objects so reopening starts a fresh session.</summary>
    public virtual void StopGame()
    {
        if (stopping)
            return;
        stopping = true;
        StopAllCoroutines();
        CancelInvoke();
        GameObject[] spawnedObjects = spawnObjects == null ? Array.Empty<GameObject>() : spawnObjects.ToArray();
        spawnObjects?.Clear();
        try
        {
            foreach (GameObject spawned in spawnedObjects)
            {
                if (spawned == null)
                    continue;
                spawned.SetActive(false);
                if (Application.isPlaying)
                    Destroy(spawned);
                else
                    DestroyImmediate(spawned);
            }
        }
        finally { stopping = false; }
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
