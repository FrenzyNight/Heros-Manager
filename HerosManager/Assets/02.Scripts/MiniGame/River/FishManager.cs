using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    private RiverCSVReader RiverCSV;
    public Vector2 SpawnPoint1, SpawnPoint2, SpawnPoint3;
    public GameObject Log1,Log2;
    public GameObject FishPrefab, GoldFishPrefab;
    private GameObject SelectedFish;

    private LogMove LM1,LM2;

    private float x, y;
    private int rnd;
    // Start is called before the first frame update
    void Start()
    {
        RiverCSV = GameObject.Find("RiverCSVReader").GetComponent<RiverCSVReader>();
        LM1 = Log1.GetComponent<LogMove>();
        LM2 = Log2.GetComponent<LogMove>();
        StartCoroutine(FishSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            LM1.direction *= -1;
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            LM2.direction *= -1;
        }
    }

    IEnumerator FishSpawn()
    {
        while(true)
        {

            rnd = Random.Range(1,101);
            if(rnd <= RiverCSV.fishGoldPer)
            {
                SelectedFish = GoldFishPrefab;
            }
            else
            {
                SelectedFish = FishPrefab;
            }
            
            rnd = Random.Range(1,4);

            if(rnd == 1)
            {
                Instantiate(SelectedFish, SpawnPoint1, Quaternion.identity);
            }
            else if(rnd == 2)
            {
                Instantiate(SelectedFish, SpawnPoint2, Quaternion.identity);
            }
            else if(rnd == 3)
            {
                Instantiate(SelectedFish, SpawnPoint3, Quaternion.identity);
            }

            yield return new WaitForSeconds(RiverCSV.fishCoolTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint1, 0.1f);
        Gizmos.DrawSphere(SpawnPoint2, 0.1f);
        Gizmos.DrawSphere(SpawnPoint3, 0.1f);
    }
}
