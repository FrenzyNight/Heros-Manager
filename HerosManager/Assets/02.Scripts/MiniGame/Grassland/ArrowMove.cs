using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    private HuntManager HM;

    private float realArrowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("HuntManager").GetComponent<HuntManager>();
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(HM.realArrowSpeed * Time.deltaTime,0,0);
    }
}
