using UnityEngine;using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {
	
	private bool zPressed;
	private bool xPressed;
	private GameObject selectedObject;
	private Predict predictor;
	private WiimoteScript wiimote;
	
	public GameObject cursor;
	
	//TODO: make another cursor - starts from the center and attach it to this script
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
		Vector3 scale = selectedObject.transform.localScale;
		float accY = wiimote.accY;
		
		//scale += accY;
		selectedObject.transform.localScale = scale;
	}
	
	//Ray casting?
	void TranslateConstantRadius () {
	}
	
	void TranslateToMe () {
		
	}

	void MoveCursor () {
		Vector3 newPosition = cursor.transform.localPosition;
		newPosition.x -= wiimote.accX * 0.25f;
		newPosition.z += wiimote.accY * 0.25f;
		cursor.transform.localPosition = Vector3.Slerp (cursor.transform.localPosition, newPosition, Time.deltaTime * 20);
	}
	
	// Use this for initialization
	void Start () {
		cursor.transform.localPosition = new Vector3 (0, 0, 1);
		
		predictor = GameObject.FindGameObjectWithTag ("Predictor").GetComponent<Predict>();
		wiimote = GameObject.FindGameObjectWithTag("Wiimote").GetComponent<WiimoteScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Z)) {
			
			if (!zPressed) {
				//TODO: make the cursor appear and move around
				//first time pressing Z, place cursor in center of canvas
				cursor.transform.localPosition = new Vector3((float)-((cursor.GetComponent<Renderer>()).bounds.size.x/2),
				                                             (float)0.1,
				                                             (float)((cursor.GetComponent<Renderer>()).bounds.size.y/2));
				cursor.GetComponent<Renderer>().enabled = true;
				zPressed = true;
				
				
			} else {
				//Z has been pressed for a while start moving the cursor
				//TODO: move the cursor around
				MoveCursor ();
			}
			if (Input.GetKey (KeyCode.X)) {
				//TODO: select an object - raycast to collide with an object
			}
		}
		if (!Input.GetKey (KeyCode.Z)) {
			//TODO: deselect the object
			//hide cursor
			zPressed = false;
			cursor.GetComponent<Renderer> ().enabled = false;
		}
		if (!Input.GetKey (KeyCode.X)) xPressed = false;
	}
}