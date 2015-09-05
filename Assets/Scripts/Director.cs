using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {

	public string animation = "Sit 03";

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<Animator> ().Play (animation);	
	}
}
