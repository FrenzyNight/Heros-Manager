using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OasisCharMove : MonoBehaviour
{   
    private OasisManager OM;
    private RectTransform rt;
    public GameObject charObj;

    public GameObject NetPrefab;
    bool isCoolTime;
    private Quaternion initialRotation;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        initialRotation = rt.localRotation;
    }

    void OnEnable()
    {
        rt.localRotation = initialRotation;
        isCoolTime = false;
    }

    void Start()
    {
        OM = MiniGameReferences.Manager<OasisManager>(this);

        rt = gameObject.GetComponent<RectTransform>();

        isCoolTime = false;
    }


    void Update()
    {
        if(GamepadUiController.MiniGameInputAllowed && (Input.GetKey(KeyCode.Space) || GamepadInput.Held(GamepadButton.Confirm)) && !isCoolTime)
        {
            StartCoroutine(ThrowNet());
        }
    }

    void FixedUpdate()
    {
        rt.Rotate(new Vector3(0,0,1) * OM.realOasisCharSpeed * Time.deltaTime);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        isCoolTime = false;
    }

    IEnumerator ThrowNet()
    {
        isCoolTime = true;
        GamepadInput.Pulse(0.08f, 0.18f, 0.06f);
        GameObject obj;
        obj = OM.Spawn(NetPrefab, charObj.transform.position, Quaternion.identity, MiniGameReferences.Container(this, "Oasis"));
        obj.GetComponent<RectTransform>().Rotate(new Vector3(0,0,transform.eulerAngles.z));

        yield return new WaitForSeconds(1f);

        isCoolTime = false;
    }

    
}
