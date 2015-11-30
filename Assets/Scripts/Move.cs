using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public float speed = 0.5f;
	private Camera thisCamera;
	private bool activated;

	void Awake () {
		thisCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		if (activated) {
			float h = Input.GetAxisRaw ("Horizontal");
			float v = Input.GetAxisRaw ("Vertical");
			Vector3 newPos = Vector3.Lerp (transform.position, transform.position + (h * thisCamera.transform.right) + (v * thisCamera.transform.forward), speed);
			if (newPos.y >= 0)
				//newPos.y = transform.position//0;
				transform.position = newPos;
			else
				print (newPos);
		}
	}

	public void SetActivated(bool active) {
		activated = active;
	}
}
