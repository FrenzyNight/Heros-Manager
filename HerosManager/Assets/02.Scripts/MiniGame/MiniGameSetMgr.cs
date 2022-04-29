using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameSetMgr : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        RealSpawnPoint = new Vector2(BlockSpawnPoint.x * widthScale + transform.position.x , BlockSpawnPoint.y * heightScale + transform.position.y);
    }

    public virtual void StartGame() {}

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
