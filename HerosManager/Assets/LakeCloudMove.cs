using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LakeCloudMove : MonoBehaviour
{
	public GameObject Pair;
	private RectTransform rt;
	public float speed;

	// UI 대응

	private float moveLength;


	

	// Use this for initialization
	void Start () 
	{
		rt = gameObject.GetComponent<RectTransform>();
		
		moveLength = rt.rect.width  * transform.localScale.x;

		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (speed * Time.deltaTime) , rt.anchoredPosition.y);
         
        if(rt.anchoredPosition.x <= - moveLength)
        {
            rt.anchoredPosition = new Vector2(Pair.GetComponent<RectTransform>().anchoredPosition.x + (moveLength) - (speed * Time.deltaTime), rt.anchoredPosition.y);
        }
		
	}
}
