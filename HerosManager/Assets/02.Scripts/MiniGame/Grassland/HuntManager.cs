using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HuntManager : MonoBehaviour
{
    private GrasslandCSVReader GrassCSV;
    public GameObject FoxPrefab, BearPrefab;
    public Vector2 StartPoint, EndPoint;
    public Vector2 StartPoint2, EndPoint2;
    private float x, y;
    public int foxNum;
    public int bearNum;
    private int rnd;

    // Start is called before the first frame update
    void Start()
    {
        foxNum = 0;
        bearNum = 0;
        GrassCSV = GameObject.Find("GrasslandCSVReader").GetComponent<GrasslandCSVReader>();
        StartCoroutine(SpawnMonster());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnMonster()
    {
        while(true)
        {
            rnd = Random.Range(1,3);
            if(rnd == 1)
            {
                x = Random.Range(StartPoint.x, EndPoint.x);
                y = Random.Range(StartPoint.y, EndPoint.y);
            }
            else
            {
                x = Random.Range(StartPoint2.x, EndPoint2.x);
                y = Random.Range(StartPoint2.y, EndPoint2.y);
            }

            rnd = Random.Range(1,101);

            if(foxNum < GrassCSV.huntMonsterMaxNum && bearNum < GrassCSV.huntMonsterMaxNum)
            {
                if(rnd <= GrassCSV.huntFoxPer)
                {
                    //fox Spawn
                    Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity);
                    foxNum++;
                }
                else
                {
                    //bearSpawn
                    Instantiate(BearPrefab, new Vector2(x,y), Quaternion.identity);
                    bearNum++;
                }
            }
            else if(foxNum < GrassCSV.huntMonsterMaxNum && bearNum == GrassCSV.huntMonsterMaxNum)
            {
                //fox
                Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity);
                foxNum++;
            }

            else if(foxNum == GrassCSV.huntMonsterMaxNum && bearNum < GrassCSV.huntMonsterMaxNum)
            {
                //bear
                Instantiate(BearPrefab, new Vector2(x,y), Quaternion.identity);
                bearNum++;
            }

            yield return new WaitForSeconds(GrassCSV.huntMonsterCoolTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 0.1f);
        Gizmos.DrawSphere(EndPoint, 0.1f);
        Gizmos.DrawSphere(StartPoint2, 0.1f);
        Gizmos.DrawSphere(EndPoint2, 0.1f);
    }

}
