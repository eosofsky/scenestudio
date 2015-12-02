using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {
	
	private GameObject selectedObject;
	private WiimoteScript wiimote;
	public int mode;
	private bool justStarted;
	
	private Vector3 initialPos;
	private Vector3 center;
	private float radius;
	private Vector3 initDirection;

	private int modes = 5;
	
	/*
	 * Press - and + to toggle between modes:
	 * 1) scaling
	 * 2) translate left and right
	 * 3) translate forward and backward 
	 * 4) translate up and down
	 * 5) rotate
	 */
	
	/* These functions all assume that there is already a selected object. */
	void Scale () {
		if (selectedObject) {
			justStarted = false;

			//make bigger - with minimum and maximum scales
			Vector3 scale = selectedObject.transform.localScale;
			float m = wiimote.accY * 0.25f;
			scale -= new Vector3 (m, m, m);
			if (scale.x > 0.25f && scale.x < 2f) {
				selectedObject.transform.localScale = scale;

				//reposition on the y-axis so not underground
				Vector3 pos = selectedObject.transform.position;
				pos.y = selectedObject.GetComponent<Collider> ().bounds.size.y / 2;
				selectedObject.transform.position = pos;
			}
		}
	}

	void Rotate() {
		if (selectedObject) {
			justStarted = false;

			selectedObject.transform.RotateAround (selectedObject.transform.position, new Vector3 (0, 1, 0), wiimote.accX * 5);
		}
	}

	void TranslateOrthogonal () {
		if (selectedObject) {
			if (justStarted) {
				initialPos = selectedObject.transform.position;
				center = Camera.main.transform.position;
				center.y = initialPos.y;
				initDirection = initialPos - center;
				initDirection = new Vector3 (-initDirection.z, initDirection.y, initDirection.x);
			}
			justStarted = false;

			Vector3 pos = selectedObject.transform.position;
			selectedObject.transform.position = pos + initDirection.normalized * (-wiimote.accX) * 0.4f;
		}
	}
	
	void TranslateToMe () {
		if (selectedObject) {
			if (justStarted) {
				initialPos = selectedObject.transform.position;
				center = Camera.main.transform.position;
				center.y = initialPos.y;
				initDirection = initialPos - center;
			} 
			justStarted = false;

			//lifting the wiimote should make the object come closer to you
			Vector3 pos = selectedObject.transform.position;
			selectedObject.transform.position = pos + initDirection.normalized * (wiimote.accY) * 0.4f;
		}
	}

	void UpAndDown() {
		if (selectedObject) {
			if (justStarted) {
				initDirection = new Vector3(0,1,0);
			}
			justStarted = false;

			Vector3 pos = selectedObject.transform.position;
			selectedObject.transform.position = pos + initDirection.normalized * wiimote.accY * 0.4f;
		}
	}

	public void Delete() {
		GameObject obj = selectedObject;
		ChangeSelectedObject (null);
		Destroy (obj);
	}
	
	public void ChangeSelectedObject(GameObject obj) {
		selectedObject = obj;
	}
	
	public int ChangeMode(bool next) {
		if (next) {
			mode = (mode + 1) % modes;
		} else {
			mode = (mode - 1 + modes) % modes;
		}
		justStarted = true;
		return mode;
	}
	
	// Use this for initialization
	void Start () {
		wiimote = GameObject.FindGameObjectWithTag("Wiimote").GetComponent<WiimoteScript>();
		
		mode = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//if there is a selected object, do something
		if (selectedObject) {
			if (mode == 0) {
				Scale ();
			} else if (mode == 1) {
				TranslateOrthogonal();
			} else if (mode == 2) {
				TranslateToMe();
			} else if (mode == 3) {
				UpAndDown();
			} else {
				Rotate();
			}
		}
	}
}