using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubCharMove : MonoBehaviour
{
    private HubManager HM;

    private Transform tr;
    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("HubManager").GetComponent<HubManager>();
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            tr.Translate(Vector2.up * HM.realHubCharSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A))
        {
            tr.Translate(Vector2.left * HM.realHubCharSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S))
        {
            tr.Translate(Vector2.down * HM.realHubCharSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D))
        {
            tr.Translate(Vector2.right * HM.realHubCharSpeed * Time.deltaTime);
        }

    }
}
