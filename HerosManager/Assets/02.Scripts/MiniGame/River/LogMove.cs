using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMove : MonoBehaviour
{
    private RiverCSVReader RiverCSV;
    private float realLogSpeed;
    public float direction;

    //public Vector2 limitUp, limitDown;
    // Start is called before the first frame update
    void Start()
    {
        RiverCSV = GameObject.Find("RiverCSVReader").GetComponent<RiverCSVReader>();
        realLogSpeed = RiverCSV.standardFishSpeed * RiverCSV.fishLogSpeed;
        direction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0,realLogSpeed * direction * Time.deltaTime,0);

        //Debug.Log("position y = " + transform.position.y.ToString());
        
        
        if(transform.position.y >= 18)
        {
            direction = -1;
        }
        else if(transform.position.y <= 12)
        {
            direction = 1;
        }
        
    }
    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(limitUp, 0.1f);
        Gizmos.DrawSphere(limitDown, 0.1f);
    }
    */
}
