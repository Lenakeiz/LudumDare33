using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		this.GetComponent<Animator> ().Play ("Sit 02");	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
