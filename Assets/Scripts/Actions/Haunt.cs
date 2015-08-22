using UnityEngine;
using System.Collections;

public class Haunt : MonoBehaviour {


	Player player;

	bool choosingDirection =false;

	public GameObject arrowPrefab;
	public GameObject hauntPrefab;

	public Vector3 hauntDirection;

	public float arrowOffset = 2.0f;

	public float hauntDuration;
	float hauntDurationTimer;
	
	public float hauntCooldown;
	public float hauntCooldownTimer;
	
	public float amountOfFear;
	
	int lockNumber;

	public bool Activate(int lockNum)
	{
		if (hauntCooldownTimer > 0 || choosingDirection )return false;


		choosingDirection = true;

		arrowPrefab.SetActive (true);
		arrowPrefab.transform.position = this.transform.position + hauntDirection * arrowOffset;
		arrowPrefab.transform.localRotation = Quaternion.LookRotation (Vector3.up,hauntDirection);

		hauntCooldownTimer = hauntCooldown;
		hauntDurationTimer = hauntDuration;
		lockNumber = lockNum;
		return true;
	}

	// Use this for initialization
	void Start () {
		if (arrowPrefab) {
			arrowPrefab = GameObject.Instantiate<GameObject>(arrowPrefab);
			arrowPrefab.SetActive (false);
		} else {
			Debug.LogError ("NO ARROW PREFAB FOR HAUNT");
		}

		if (hauntPrefab) {
			hauntPrefab = GameObject.Instantiate<GameObject>(hauntPrefab);
			hauntPrefab.SetActive (false);
		} else {
			Debug.LogError ("NO HAUNT PREFAB FOR HAUNT");
		}
		player = this.GetComponent<Player> ();
		if (player == null) {
			Debug.LogError("CANT FIND PLAYER");
		}
		hauntDirection = new Vector3 (0, 0, 1);
	}
	// Update is called once per frame
	void Update ()
	{
		if (hauntCooldownTimer > 0) {
			hauntCooldownTimer -= Time.deltaTime;
			if(hauntCooldown < 0)
			{
				hauntCooldownTimer = 0;
			}
		
		}

		if (choosingDirection) {
			if (Input.GetButtonUp ("Fire2")) {
				//release and spawn the haunt
				hauntPrefab.SetActive(true);
				hauntPrefab.transform.position = this.transform.position;
				hauntPrefab.transform.localRotation = Quaternion.LookRotation(hauntDirection,Vector3.up);
				arrowPrefab.gameObject.SetActive(false);
				player.CancelAction(lockNumber);
				choosingDirection = false;
			}

			Vector3 direction = Vector3.zero;
			direction.x = Input.GetAxis ("Horizontal");
			direction.z = Input.GetAxis ("Vertical");
			if (direction.x != 0 || direction.z != 0) {
				if (Mathf.Abs (direction.z) > Mathf.Abs (direction.x)) {
					direction.x = 0;
				} else {
					direction.z = 0;
				}
				arrowPrefab.transform.position = this.transform.position + direction.normalized * arrowOffset;
				arrowPrefab.transform.localRotation = Quaternion.LookRotation (Vector3.up,direction);
				hauntDirection = direction.normalized;
			}
		}
	}
}
