using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run_Move : MonoBehaviour
{
    public GameObject Running;
    public GameObject Sliding;

    private float jump = 10f;
    private float jump2 = 9f;

    private int jumpcount = 0;

    private Rigidbody2D vel;

    public void Jump()
    {
        if(jumpcount == 0)
        {
            Run();
            vel.velocity = new Vector3(0, jump, 0);
            jumpcount += 1;
        }
        else if(jumpcount == 1)
        {
            vel.velocity = new Vector3(0, jump2, 0);
            jumpcount += 1;
        }
    }

    public void Slide()
    {
        if(jumpcount == 0)
        {
            Running.SetActive(false);
            Sliding.SetActive(true);
        }
    }

    public void Run()
    {
        Running.SetActive(true);
        Sliding.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {            
        if(collision.gameObject.tag == "Land")
        {
            jumpcount = 0;
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        vel = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        if(Input.GetKey(KeyCode.DownArrow))
        {
            Slide();
        }

        if(Input.GetKeyUp(KeyCode.DownArrow))
        {
            Run();
        }
    }
}
