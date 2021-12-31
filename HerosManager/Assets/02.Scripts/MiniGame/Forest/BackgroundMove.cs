using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour {



	public float speed;
	private float x;
	public float ResetPoint;
	public float OriginalPoint;

	private CaveManager CM;

	// Use this for initialization
	void Start () 
	{
		CM = GameObject.Find("CaveManager").GetComponent<CaveManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
			if(CM.isRun)
			{
				x = transform.position.x;
				x += speed * Time.deltaTime;
				transform.position = new Vector3 (x, transform.position.y, transform.position.z);

				if (x <= ResetPoint)
				{
				x = OriginalPoint;
				transform.position = new Vector3 (x, transform.position.y, transform.position.z);
				}

		}
	}
}
