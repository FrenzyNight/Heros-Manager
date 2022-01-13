using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMove : MonoBehaviour
{
	public GameObject Pair;
	private RectTransform rt;
	public float speed;

	private CaveManager CM;

	// UI 대응

	private float moveLength;


	

	// Use this for initialization
	void Start () 
	{
		CM = GameObject.Find("CaveManager").GetComponent<CaveManager>();
		rt = gameObject.GetComponent<RectTransform>();
		

		moveLength = rt.rect.width  * transform.localScale.x;

		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(CM.isRun)
		{
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (speed * Time.deltaTime) , rt.anchoredPosition.y);
			//transform.Translate( -speed * resolutionScale * Time.deltaTime , 0, 0);
			
			if(rt.anchoredPosition.x <= - moveLength)
			{
				rt.anchoredPosition = new Vector2(Pair.GetComponent<RectTransform>().anchoredPosition.x + (moveLength) - (speed * Time.deltaTime), rt.anchoredPosition.y);
			}
		}
	}
}
