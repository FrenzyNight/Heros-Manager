﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAngle : MonoBehaviour
{
    private GrasslandCSVReader GrassCSV;
    public GameObject ArrowPrefab;
    private float angle;
    private Vector2 target, mouse;
    private Camera cam;

    private bool isShoot;


    // Start is called before the first frame update
    void Start()
    {
        GrassCSV = GameObject.Find("GrasslandCSVReader").GetComponent<GrasslandCSVReader>();
        isShoot = false;
        target = transform.position;
        cam = GameObject.Find("HuntCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle - 45, Vector3.forward);
    
        if(Input.GetMouseButtonDown(0) && !isShoot)
        {
            StartCoroutine("ShootArrow");
        }
    }

    IEnumerator ShootArrow()
    {
        isShoot = true;
        Instantiate(ArrowPrefab, target, Quaternion.AngleAxis(angle, Vector3.forward));

        yield return new WaitForSeconds(GrassCSV.huntArrowCoolTime);
        isShoot = false;

        yield return null;

    }
}
