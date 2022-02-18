using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAngle : MonoBehaviour
{
    private HuntManager HM;
    public GameObject ArrowPrefab;
    private float angle;
    private Vector2 target, mouse;
    


    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("HuntManager").GetComponent<HuntManager>();
        target = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        mouse = Input.mousePosition;
        angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
        
        this.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        //this.transform.rotation = Quaternion.Euler(0,0,angle-45);
        if(Input.GetMouseButtonDown(0) && !HM.isShoot)
        {
            StartCoroutine("ShootArrow");
        }
    }

    IEnumerator ShootArrow()
    {
        HM.isShoot = true;
        Instantiate(ArrowPrefab, target, Quaternion.AngleAxis(angle, Vector3.forward), GameObject.Find("Hunt").transform);

        gameObject.GetComponent<Animator>().Play("Bow_ani", -1, 0f);
        yield return new WaitForSeconds(HM.huntArrowCoolTime);
        HM.isShoot = false;

        yield return null;

    }
}
