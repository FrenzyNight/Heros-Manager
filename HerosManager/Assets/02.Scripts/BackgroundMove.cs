using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour {



	public float speed;
	private float x;
	public float ResetPoint;
	public float OriginalPoint;



	// Use this for initialization
	void Start () {
		//OriginalPoint = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {


		x = transform.position.x;
		x += speed * Time.deltaTime;
		transform.position = new Vector3 (x, transform.position.y, transform.position.z);



		if (x <= ResetPoint){

			//Debug.Log ("hhhh");
			x = OriginalPoint;
			transform.position = new Vector3 (x, transform.position.y, transform.position.z);
		}


	}
}
