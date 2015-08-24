using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {


	public string failAnimationName;
	public string winAinimationName;
	GameObject player; 
	GameObject mainCamera;
	Vector3 originalCameraPos;


	
	public float zoomedDistance =2.0f;
	public float zoomSpeedMultiplier = 0.5f;
	bool zoomCamera;
	float t=0;





	public void GameOverActivate(bool didWeWin)
	{
		t = 0;
		player = GameObject.FindObjectOfType<Player> ().gameObject;
		player.GetComponent<Player> ().enabled = false;

		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (!mainCamera) {
			Debug.LogError("NO camera");
		}
		originalCameraPos = mainCamera.transform.position;


		player.transform.rotation = Quaternion.LookRotation (new Vector3 (originalCameraPos.x, 1, originalCameraPos.z));

		Actors[] actors = GameOver.FindObjectsOfType<Actors> ();
		foreach (Actors a in actors) {
			a.enabled=false;
		}
		zoomCamera = true;
		if (didWeWin) {
			player.GetComponent<Animator> ().Play (winAinimationName);
		} else {
			player.GetComponent<Animator> ().Play (failAnimationName);
		}
	}

	public void Reset()
	{
		zoomCamera = false;
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (zoomCamera) {
			if(player != null)
			{
				Vector3 camToPlayer = player.transform.position - mainCamera.transform.position;
				if(camToPlayer.magnitude > zoomedDistance)
				{
					t += Time.deltaTime;
					mainCamera.transform.position = Vector3.Lerp(originalCameraPos,player.transform.position,t * zoomSpeedMultiplier);
				}
				else{
					zoomCamera = false;
				}
			}
		}
	}
}
