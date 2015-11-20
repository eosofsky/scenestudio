using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public float speed = 0.5f;
	private Camera thisCamera;

	void Awake () {
		thisCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		Vector3 newPos = Vector3.Lerp (transform.position, transform.position + (h * thisCamera.transform.right) + (v * thisCamera.transform.forward), speed);
		if (newPos.y >= 0)
			//newPos.y = transform.position//0;
			transform.position = newPos;
		else
			print(newPos);
	}
}
