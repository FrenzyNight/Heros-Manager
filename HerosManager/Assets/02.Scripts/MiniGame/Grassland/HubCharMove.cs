using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubCharMove : MonoBehaviour
{
    private GrasslandCSVReader GrassCSV;

    private float realSpeed;
    private Transform tr;
    // Start is called before the first frame update
    void Start()
    {
        GrassCSV = GameObject.Find("GrasslandCSVReader").GetComponent<GrasslandCSVReader>();
        tr = GetComponent<Transform>();
        realSpeed = GrassCSV.hubCharSpeed * GrassCSV.standardHubSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            tr.Translate(Vector2.up * realSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A))
        {
            tr.Translate(Vector2.left * realSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S))
        {
            tr.Translate(Vector2.down * realSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D))
        {
            tr.Translate(Vector2.right * realSpeed * Time.deltaTime);
        }

    }
}
