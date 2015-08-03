using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Mono.Data.SqliteClient;

public class MoveWithMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		previousMousePosition = currentMousePosition = Vector3.zero;
		move = false;
		IsMoving = false;
	}
	
	// Update is called once per frame

	Vector3 previousMousePosition;
	Vector3 currentMousePosition;
	bool move;
	bool IsMoving;

	Vector3 moveTo;
	Quaternion rotateTo;
	float timeLeft;




	void TriggerMoveTo(Vector3 target, Quaternion rotateTo, float time)
	{
		moveTo = target;
		timeLeft = time;
		this.rotateTo = rotateTo;

		IsMoving = true;
	}

	void Update () {

		if (IsMoving && timeLeft >= 0)
		{
			/*float distanceX = moveTo.x - transform.position.x;
			float distanceY = moveTo.y - transform.position.y;
			float distanceZ = moveTo.z - transform.position.z;
			float distanceTotal = Vector3.Distance(moveTo, transform.position);

			float speedX = distanceX / timeLeft;
			float speedY = distanceY / timeLeft;
			float speedZ = distanceZ / timeLeft;
			float speedTotal = distanceTotal / timeLeft; 
			*/
			float deltaTime = Time.deltaTime;
			//transform.position = new Vector3(transform.position.x + speedX * deltaTime, transform.position.y + speedY * deltaTime, transform.position.z + speedZ * deltaTime);

			transform.position = Vector3.Lerp(transform.position, moveTo, deltaTime / timeLeft);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, deltaTime / timeLeft);

			timeLeft -= deltaTime;

	
		}
		else
		{
			IsMoving = false;

		}




		previousMousePosition = currentMousePosition;
		currentMousePosition = Input.mousePosition;
		var ray = Camera.main.ScreenPointToRay(currentMousePosition);

		RaycastHit hit = new RaycastHit();

		if (Input.GetMouseButton(0))
		{
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.gameObject == gameObject)
				{
					if (move == false)
					{
						/*Vector3 prev = Camera.main.ScreenToWorldPoint(new Vector3(previousMousePosition.x, previousMousePosition.y, Camera.main.transform.position.y-5));
						*/Vector3 curr = Camera.main.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, Camera.main.transform.position.y-5));

						TriggerMoveTo(new Vector3(curr.x, 5, curr.z), Quaternion.identity, 0.5f);
					}
					move = true;
				}
			}
		}
		else
		{
			if (move == true)
			{
				TriggerMoveTo(new Vector3(transform.position.x, 1, transform.position.z), Quaternion.Euler(0, 0, 0),  0.5f);
			}
			move = false;

		}

		if (move)
		{
			/*Vector3 prev = Camera.main.ScreenToWorldPoint(new Vector3(previousMousePosition.x, previousMousePosition.y, Camera.main.transform.position.y-5));
			*/Vector3 curr = Camera.main.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, Camera.main.transform.position.y-5));

			TriggerMoveTo(new Vector3(curr.x, 5, curr.z),  Quaternion.identity, 0.1f);
			//transform.position = new Vector3(transform.position.x + (curr.x - prev.x) * Time.deltaTime * 50, 5, transform.position.z + (curr.z - prev.z) * Time.deltaTime * 50);
			//DebugThis (prev.ToString());
		}

	}
	
}
