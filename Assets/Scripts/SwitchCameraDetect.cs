using UnityEngine;
using System.Collections;

public class SwitchCameraDetect : MonoBehaviour {

	public int RoomIndex;

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			GameObject.Find("Camera").GetComponent<LevelController>().ChangeMiniCamera(RoomIndex);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
