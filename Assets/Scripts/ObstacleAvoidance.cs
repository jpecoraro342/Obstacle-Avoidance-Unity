using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObstacleAvoidance : MonoBehaviour {

	public float speed = 3.0f;
	public float avoidAmount = 20.0f;
	public float distanceToLookAhead = 5.0f;
	public float sideRayDistanceToLookAhead = 1.0f;

	public float xzbounds = 10f;

	public Text debugText;

	public GameObject targetObject;

	private Rigidbody playerRigidBody;

	private float currentSpeed;
	private Vector3 targetPoint;

	private Vector3 obstacleHit;
	private float obstacleDistance;

	private bool reachedTarget;

	private float debugRayTime = .2f;

	//private int numCollisions = 0;

	// Use this for initialization
	void Start () {
		targetPoint = Vector3.zero;
		targetPoint.y = .5f;
		reachedTarget = true;
		playerRigidBody = GetComponent<Rigidbody>();
		debugText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		//Get New Target
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 100.0f)) {
			if (PointIsInBounds(hit.point)) {
				targetPoint = hit.point;
				targetPoint.y = .5f;
				SpawnTarget(targetPoint);

				reachedTarget = false;
			}
		}
		//End Get New Target

		//Reached Target?
		if (reachedTarget) {
			ResetPosition();
			return;
		}
		//End Reached Target


		Vector3 TargetDirection = (targetPoint - transform.position);
		TargetDirection.Normalize();

		CheckForObstacles(ref TargetDirection);

		//Choose New Rotation
		Quaternion rotation = Quaternion.LookRotation(TargetDirection);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5.0f * Time.deltaTime);

		//Move Forward
		currentSpeed = speed * Time.deltaTime;
		transform.position += transform.forward * currentSpeed;

		//UpdateDebugText();
	}

	void CheckForObstacles(ref Vector3 TargetDirection) {
		RaycastHit hit;
		int layerMask = 1<<8;

		//Forward Raycast
		if (Physics.Raycast(transform.position, transform.forward, out hit, distanceToLookAhead, layerMask)) {
			Debug.DrawLine(transform.position, hit.point, Color.green, debugRayTime);
			Debug.DrawLine(hit.point, hit.normal, Color.green, debugRayTime);
			TargetDirection += hit.normal * avoidAmount;
		} 
		//End Forward Raycast

		//Left Raycast
		Vector3 Left = transform.position;
		Left.x -= .5f;
		if (Physics.Raycast(Left, transform.forward, out hit, sideRayDistanceToLookAhead, layerMask)) {
			Debug.DrawLine(Left, hit.point, Color.red, debugRayTime);
			Debug.DrawLine(hit.point, hit.normal, Color.red, debugRayTime);
			TargetDirection += hit.normal * avoidAmount;
		}
		//End Left Raycast

		//Right Raycast
		Vector3 Right = transform.position;
		Right.x += .5f;
		if (Physics.Raycast(Right, transform.forward, out hit, sideRayDistanceToLookAhead, layerMask)) {
			Debug.DrawLine(Right, hit.point, Color.blue, debugRayTime);
			Debug.DrawLine(hit.point, hit.normal, Color.blue, debugRayTime);
			TargetDirection += hit.normal * avoidAmount;
		}
		//End Right Raycast
	}

	bool PointIsInBounds(Vector3 point) {
		return point.x > -1*xzbounds && point.x < xzbounds && point.z > -1*xzbounds && point.z < xzbounds;
	}

	void OnTriggerEnter (Collider other) {
		Destroy(other.gameObject);
		reachedTarget = true;
	}

	void SpawnTarget(Vector3 SpawnLocation) {
		SpawnLocation.y = 1;

		GameObject newTarget = Instantiate(targetObject);
		newTarget.transform.position = SpawnLocation;
	}

	void ResetPosition() {
		Vector3 position = transform.position;
		position.y = .5f;
		transform.position = position;
		playerRigidBody.velocity = Vector3.zero;
	}

	void OnCollisionEnter (Collision collision) {
		//numCollisions++;
	}

	void UpdateDebugText() {
		debugText.text = "Current Location: " + transform.position + "\nTarget Location: " + targetPoint;  
			//"\n\nNumber of Collisions: " + numCollisions;
	}	
}