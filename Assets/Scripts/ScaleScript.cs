using UnityEngine;using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {
	
	private bool zPressed;
	private bool xPressed;
	private GameObject selectedObject;
	private Predict predictor;
	private WiimoteScript wiimote;
	private int mode;
	
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
		//make bigger
		Vector3 scale = selectedObject.transform.localScale;
		float m = wiimote.accY * 0.25f;
		scale += new Vector3(m,m,m);
		selectedObject.transform.localScale = scale;

		//reposition on the y-axis so not underground
		Vector3 pos = selectedObject.transform.position;
		pos.y = selectedObject.GetComponent<Renderer> ().bounds.size.y/2;
		selectedObject.transform.position = pos;
	}
	
	//Ray casting?
	void TranslateConstantRadius () {
	}
	
	void TranslateToMe () {
		
	}

	void MoveCursor () {
		Vector3 newPosition = cursor.transform.localPosition;
		newPosition.x += wiimote.accX * 0.25f;
		newPosition.y -= wiimote.accY * 0.25f;
		cursor.transform.localPosition = Vector3.Slerp (cursor.transform.localPosition, newPosition, Time.deltaTime * 20);
	}

	public void ChangeSelectedObject(GameObject obj) {
		selectedObject = obj;
	}
	
	// Use this for initialization
	void Start () {
		cursor.transform.localPosition = new Vector3 (0, 0, 3);
		
		predictor = GameObject.FindGameObjectWithTag ("Predictor").GetComponent<Predict>();
		wiimote = GameObject.FindGameObjectWithTag("Wiimote").GetComponent<WiimoteScript>();
	}
	
	// Update is called once per frame
	void Update () {
		//if there is a selected object, do something
		if (selectedObject) {
		}
	}
}