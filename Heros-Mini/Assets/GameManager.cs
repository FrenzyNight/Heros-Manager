using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Downblock;
    public GameObject Upblock;
    public GameObject Jewel;

    public Vector2 SpawnPoint;

    private int rnd;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBlock());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnBlock()
    {
        while(true)
        {
            rnd = Random.Range(1,101);

            if(rnd>=99)
            {
                Instantiate(Jewel, SpawnPoint, Quaternion.identity);
            }
            else if(rnd >= 89)
            {
                Instantiate(Upblock, SpawnPoint, Quaternion.identity);
            }
            else if(rnd >= 81)
            {
                Instantiate(Downblock, SpawnPoint, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint, 0.1f);
    }
}
