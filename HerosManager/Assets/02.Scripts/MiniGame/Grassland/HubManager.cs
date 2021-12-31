using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    private GrasslandCSVReader GrassCSV;
    public GameObject Hub,GoldHub;
    public Vector2 StartPoint, EndPoint;
    private float x, y;
    private int rnd;

    void Start()
    {
        GrassCSV = GameObject.Find("GrasslandCSVReader").GetComponent<GrasslandCSVReader>();
        StartCoroutine(SpawnHub());
    }

    void Update()
    {
        
    }

    IEnumerator SpawnHub()
    {
        while(true)
        {
            x = Random.Range(StartPoint.x, EndPoint.x);
            y = Random.Range(StartPoint.y, EndPoint.y);

            rnd = Random.Range(1, 101);
            if(rnd <= GrassCSV.hubGoldPer)
            {
                Instantiate(GoldHub, new Vector2(x,y), Quaternion.identity);
            }
            else
            {
                Instantiate(Hub, new Vector2(x,y), Quaternion.identity);
            }
            
            yield return new WaitForSeconds(GrassCSV.hubCoolTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 0.1f);
        Gizmos.DrawSphere(EndPoint, 0.1f);
    }
}
