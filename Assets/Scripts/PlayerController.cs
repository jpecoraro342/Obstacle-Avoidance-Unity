using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speedMultiplier;

	void Start() {
	}
	
	void FixedUpdate () {
		float moveHorizontal;
		float moveVertical;

		moveHorizontal = Input.GetAxis("Horizontal");
		moveVertical = Input.GetAxis("Vertical");
		
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		
		GetComponent<Rigidbody>().AddForce(movement * speedMultiplier * Time.deltaTime);
	
	}
}
