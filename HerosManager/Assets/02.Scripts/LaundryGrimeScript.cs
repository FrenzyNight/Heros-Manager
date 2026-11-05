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
        if (Time.timeScale <= 0f || laundryMgr == null || !laundryMgr.laundryUIPanel.activeInHierarchy ||
            (TutorialMgr.IsCreateInstance() && TutorialMgr.Instance != null && TutorialMgr.Instance.isTutorial)) return;
        laundryMgr.L1GrimeClick();
        GamepadInput.Pulse(0.08f, 0.2f, 0.06f);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
