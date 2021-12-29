using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HuntManager : MonoBehaviour
{
    private GrasslandCSVReader GrassCSV;

    // Start is called before the first frame update
    void Start()
    {
        GrassCSV = GameObject.Find("GrasslandCSVReader").GetComponent<GrasslandCSVReader>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
