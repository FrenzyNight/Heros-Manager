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

    void Start()
    {
        OM = GameObject.Find("OasisManager").GetComponent<OasisManager>();

        rt = gameObject.GetComponent<RectTransform>();

        isCoolTime = false;
    }


    void Update()
    {
        if(Input.GetKey(KeyCode.Space) && !isCoolTime)
        {
            StartCoroutine(ThrowNet());
        }
    }

    void FixedUpdate()
    {
        rt.Rotate(new Vector3(0,0,1) * OM.realOasisCharSpeed * Time.deltaTime);
    }

    IEnumerator ThrowNet()
    {
        isCoolTime = true;
        GameObject obj;
        obj = Instantiate(NetPrefab, charObj.transform.position, Quaternion.identity, GameObject.Find("Oasis").transform);
        obj.GetComponent<RectTransform>().Rotate(new Vector3(0,0,transform.eulerAngles.z));

        yield return new WaitForSeconds(1f);

        isCoolTime = false;
    }

    
}
