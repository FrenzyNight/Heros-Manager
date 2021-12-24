using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAngle : MonoBehaviour
{
    public GameObject ArrowPrefab;
    private float angle;
    private Vector2 target, mouse;



    // Start is called before the first frame update
    void Start()
    {
        target = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle - 45, Vector3.forward);
    
        if(Input.GetMouseButtonDown(0))
        {
            Instantiate(ArrowPrefab, target, Quaternion.AngleAxis(angle, Vector3.forward));
        }
    }
}
