using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    private GrasslandCSVReader GrassCSV;

    private float realArrowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        GrassCSV = GameObject.Find("GrasslandCSVReader").GetComponent<GrasslandCSVReader>();
        realArrowSpeed = GrassCSV.standardHuntSpeed * GrassCSV.huntArrowSpeed;
        Destroy(gameObject, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(realArrowSpeed * Time.deltaTime,0,0);
    }
}
