using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMove : MiniGameObjectMgr
{
	private CaveManager Mgr;
	public GameObject Pair;
	private RectTransform rt;
	public float speed;


	private float moveLength;



	// Use this for initialization
	void Start () 
	{
		if(manager != null)
			Mgr = manager.GetComponent<CaveManager>();
		rt = gameObject.GetComponent<RectTransform>();

		moveLength = rt.rect.width  * transform.localScale.x;
	}
	
	void FixedUpdate () 
	{
		if(Mgr != null)
		{
			if(Mgr.isCheck)
			{
				rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (speed * Time.deltaTime) , rt.anchoredPosition.y);
				
				if(rt.anchoredPosition.x <= - moveLength)
				{
					rt.anchoredPosition = new Vector2(Pair.GetComponent<RectTransform>().anchoredPosition.x + (moveLength) - (speed * Time.deltaTime), rt.anchoredPosition.y);
				}
			}
		}
		
	}

	void Update()
	{
		if(Mgr == null)
		{
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (speed) , rt.anchoredPosition.y);
				
			if(rt.anchoredPosition.x <= - moveLength)
			{
				rt.anchoredPosition = new Vector2(Pair.GetComponent<RectTransform>().anchoredPosition.x + (moveLength) - (speed * Time.deltaTime), rt.anchoredPosition.y);
			}
		}
	}
}
