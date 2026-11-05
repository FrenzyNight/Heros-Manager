using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub2Cactus : MonoBehaviour
{
    private Hub2Manager HM;
    
    // Start is called before the first frame update
    void Start()
    {
        HM = MiniGameReferences.Manager<Hub2Manager>(this);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Hub2CharMove>().StartStun();
        }
    }
}
