using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBlock : MonoBehaviour
{
    public bool isVisit;
    public bool isPop;
    // Start is called before the first frame update
    void Awake()
    {
        isVisit = false;
        isPop = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
