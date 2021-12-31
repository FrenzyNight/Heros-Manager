using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMove : MonoBehaviour
{
    private RiverCSVReader RiverCSV;
    private float realRockSpeed;
    // Start is called before the first frame update
    void Start()
    {
        RiverCSV = GameObject.Find("RiverCSVReader").GetComponent<RiverCSVReader>();
        realRockSpeed = RiverCSV.waterRockSpeed * RiverCSV.standardWaterSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -realRockSpeed * Time.deltaTime, 0);
    }
}
