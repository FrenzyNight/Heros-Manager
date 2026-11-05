using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow2Angle : MonoBehaviour
{
    private Hunt2Manager HM;
    public GameObject ArrowPrefab;
    private float angle;
    private Vector2 target, mouse;
    


    // Start is called before the first frame update
    void Start()
    {
        HM = MiniGameReferences.Manager<Hunt2Manager>(this);
        target = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GamepadUiController.MiniGameInputAllowed) return;
        target = transform.position;
        mouse = Input.mousePosition;
        if (GamepadInput.Aim.sqrMagnitude > 0.01f)
            angle = Mathf.Atan2(GamepadInput.Aim.y, GamepadInput.Aim.x) * Mathf.Rad2Deg;
        else if (!GamepadInput.Connected || Input.GetMouseButton(0) || Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
            angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
        
        this.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        //this.transform.rotation = Quaternion.Euler(0,0,angle-45);
        if((Input.GetMouseButtonDown(0) || GamepadInput.Pressed(GamepadButton.RightTrigger)) && !HM.isShoot)
        {
            StartCoroutine("ShootArrow");
        }
    }

    IEnumerator ShootArrow()
    {
        HM.isShoot = true;
        GamepadInput.Pulse(0.08f, 0.18f, 0.06f);
        HM.Spawn(ArrowPrefab, target, Quaternion.AngleAxis(angle, Vector3.forward), MiniGameReferences.Container(this, "Hunt2"));

        gameObject.GetComponent<Animator>().Play("Bow_ani", -1, 0f);
        yield return new WaitForSeconds(HM.huntArrowCoolTime);
        HM.isShoot = false;

        yield return null;

    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (HM != null) HM.isShoot = false;
    }
}
