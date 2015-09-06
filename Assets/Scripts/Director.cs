using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {

	public string animation;
	private Camera cam;

	// Use this for initialization
	void Awake () {
		this.GetComponent<Animator> ().Play (animation);
		cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		//transform.LookAt(cam.transform.position,Vector3.up);
	}
}
