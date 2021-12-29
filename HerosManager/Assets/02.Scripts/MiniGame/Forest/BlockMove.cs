using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : MonoBehaviour
{
    private CaveManager CM;
    // Start is called before the first frame update
    void Start()
    {
        CM = GameObject.Find("CaveManager").GetComponent<CaveManager>();
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(CM.isRun)
        {
            transform.Translate(-CM.realSpeed * Time.deltaTime, 0, 0);
        }
    }
}
