using UnityEngine;using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {

	private GameObject selectedObject;
	private InterfaceManager interfaceManager;
	private WiimoteScript wiimote;
	private int mode;
	private bool justStarted;
	private bool zPressed;
	private bool oPressed;
	private bool pPressed;

	private Vector3 initialPos;
	private Vector3 center;
	private float radius;
	private Vector3 initDirection;

	/*
	 * Press 2 to toggle between 3 modes:
	 * 1) translation (constant radius around you) - rotate right and left
	 * 2) translation (closer and farther from you) - lift up (closser) and down (farther)
	 * 3) scaling - lift up (bigger) and down (smaller)
	 * 
	 * Press Z to make the cursor appear. Move the wiimote around by lifting up and twisting.
	 * Press X to select an object (whichever object you intersect first). 
	 */
	
	/* These functions all assume that there is already a selected object. */
	void Scale () {
		Debug.Log ("scaling");
		justStarted = false;

		//make bigger - with minimum and maximum scales
		Vector3 scale = selectedObject.transform.localScale;
		float m = wiimote.accY * 0.25f;
		scale -= new Vector3(m,m,m);
		if (scale.x > 0.25f && scale.x < 2f) {
			selectedObject.transform.localScale = scale;
		}

		//reposition on the y-axis so not underground
		Vector3 pos = selectedObject.transform.position;
		pos.y = (selectedObject.GetComponent<Mesh>().bounds.size.y*selectedObject.transform.localScale.y)/2;
		selectedObject.transform.position = pos;
	}

	void TranslateConstantRadius () {
		Debug.Log ("constant radius");
		if (justStarted) {
			initialPos = selectedObject.transform.position;
			center = Camera.main.transform.position;
			radius = Vector3.Distance(initialPos,center);
		}
		justStarted = false;

		float deg = wiimote.accX * 180;
		selectedObject.transform.RotateAround (center, new Vector3 (1, 0, 1), deg);
	}
	
	void TranslateToMe () {
		Debug.Log ("translate to and from");
		float accY = wiimote.accY;

		if (justStarted) {
			initialPos = selectedObject.transform.position;
			center = Camera.main.transform.position;
			initDirection = initialPos - center;
		} 
		justStarted = false;

		//lifting the wiimote should make the object come closer to you
		Vector3 pos = selectedObject.transform.position;
		radius = Vector3.Distance (pos,center);
		radius -= accY * 0.25f;
		selectedObject.transform.position = pos + initDirection.normalized*radius;
	}

	void Delete() {
		GameObject obj = selectedObject;
		ChangeSelectedObject (null);
		Destroy (obj);
	}

	public void ChangeSelectedObject(GameObject obj) {
		if (obj && !selectedObject) {
			//TODO: ENTER EDIT MODE
		} else if (!obj && selectedObject) {
			//TODO: EXIT EDIT MODE
		}

		selectedObject = obj;
	}

	void ChangeMode(bool next) {
		if (next) {
			mode = (mode + 1) % 3;
		} else {
			mode = (mode - 1 + 3) % 3;
		}
		justStarted = true;
	}
	
	// Use this for initialization
	void Start () {
		wiimote = GameObject.FindGameObjectWithTag("Wiimote").GetComponent<WiimoteScript>();
		interfaceManager = GameObject.FindGameObjectWithTag ("Interface Manager").GetComponent<InterfaceManager> ();

		mode = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//if there is a selected object, do something
		if (selectedObject) {
			switch(mode) {
	  			case 0:
					Scale ();
					break;
				case 1:
					TranslateConstantRadius();
					break;
				default:
					TranslateToMe();
					break;
			}

			//delete object
			if (Input.GetKey (KeyCode.Z) && !zPressed) {
				zPressed = true;
				Delete ();
			}

			//change modes
			if (Input.GetKey (KeyCode.O) && !oPressed) {
				oPressed = true;
				ChangeMode(false);
			}
			if (Input.GetKey (KeyCode.P) && !pPressed) {
				pPressed = true;
				ChangeMode(true);
			}


			if (!Input.GetKey (KeyCode.Z)) zPressed = false;
			if (!Input.GetKey (KeyCode.O)) oPressed = false;
			if (!Input.GetKey (KeyCode.P)) pPressed = false;
		}
	}
}