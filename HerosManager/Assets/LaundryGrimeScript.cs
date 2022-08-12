using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaundryGrimeScript : MonoBehaviour
{
    //public float exi
    public GameObject InterMgr;
    [HideInInspector] public LaundryInteractionMgr laundryMgr;

    void Start()
    {
        
    }
    public void GrimeSet(float _time)
    {
        laundryMgr = InterMgr.GetComponent<LaundryInteractionMgr>();
        gameObject.transform.GetComponent<Button>().onClick.AddListener(GrimeClick);
        Destroy(this, _time);
    }

    public void GrimeClick()
    {
        laundryMgr.L1GrimeClick();
        Destroy(gameObject);
    }
}
