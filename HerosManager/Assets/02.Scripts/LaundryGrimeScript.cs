using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaundryGrimeScript : MonoBehaviour
{
    [HideInInspector] public LaundryInteractionMgr laundryMgr;

    void Start()
    {
        
    }
    public void GrimeSet(GameObject interMgr, float time)
    {
        laundryMgr = interMgr.GetComponent<LaundryInteractionMgr>();
        gameObject.transform.GetComponent<Button>().onClick.AddListener(GrimeClick);
        Destroy(gameObject, time);
    }

    void GrimeClick()
    {
        laundryMgr.L1GrimeClick();
        Destroy(gameObject);
    }
}
