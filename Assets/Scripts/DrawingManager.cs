using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class DrawingManager : MonoBehaviour
{
	//for Drawing
	private bool isMousePressed;
	private bool initial;
	int canvasMask;
	float camRayLength = 100f;
	Texture2D texture;
	public Transform bottomLeft;
	public Transform topRight;
	private float widthFactor;
	private float heightFactor;
	private Vector2 lastpoint;
	public Camera thisCamera;
	public bool empty;
	
	//for Wiimote
	public GameObject cursor;
	private bool zPressed;
	private bool xPressed;
	private WiimoteScript wiimote;

	void drawPoint(Vector2 point) {
		int x = (int) point.x;
		int y = (int) point.y;
		texture.SetPixel(x-1, y-1, Color.black);
		texture.SetPixel(x-1, y, Color.black);
		texture.SetPixel(x-1, y+1, Color.black);
		texture.SetPixel(x, y-1, Color.black);
		texture.SetPixel(x, y, Color.black);
		texture.SetPixel(x, y+1, Color.black);
		texture.SetPixel(x+1, y-1, Color.black);
		texture.SetPixel(x+1, y, Color.black);
		texture.SetPixel(x+1, y+1, Color.black);
	}

	Vector2 cursorTarget () {
		Vector3 currPos = cursor.transform.localPosition;
		float x = (float)(currPos.x + ((cursor.GetComponent<Renderer> ()).bounds.size.x / 2));
		float y = (float)(currPos.z - ((cursor.GetComponent<Renderer> ()).bounds.size.y / 2));
		return new Vector2 (x, y);
	}

	Vector2 cursorOnCanvas () {
		Vector2 position = cursorTarget ();
		float width = topRight.localPosition.x - bottomLeft.localPosition.x;
		float height = topRight.localPosition.z - bottomLeft.localPosition.z;

		Vector2 distance = position - new Vector2(bottomLeft.localPosition.x,bottomLeft.localPosition.z);
		float percX = distance.x / width;
		float percY = distance.y / height;

		return new Vector2 (percX * texture.width, percY * texture.height);
	}

	bool pointIsOnCanvas(Vector2 point) {
		return point.x >= 0 && point.x <= texture.width &&
			   point.y >= 0 && point.y <= texture.height;
	}

	void moveCursor () {
		Vector3 newPosition = cursor.transform.localPosition;
		newPosition.x -= wiimote.accX * 0.25f;
		newPosition.z += wiimote.accY * 0.25f;
		cursor.transform.localPosition = Vector3.Slerp (cursor.transform.localPosition, newPosition, Time.deltaTime * 20);
	}
	
	void Awake ()
	{	
		isMousePressed = false;
		zPressed = false;
		
		canvasMask = LayerMask.GetMask ("Canvas");
		
		texture = new Texture2D(256, 256);
		GetComponent<Renderer>().material.mainTexture = texture;
		
		clear ();
		
		lastpoint = new Vector2 (0,0);
		initial = true;
		
		widthFactor = (texture.width / (topRight.position.x - bottomLeft.position.x));
		heightFactor = (texture.height / (topRight.position.y - bottomLeft.position.y));
	
		wiimote = GameObject.FindGameObjectWithTag ("Wiimote").GetComponent<WiimoteScript> ();
	}

	void Update()
	{	
		//B BUTTON
		if (Input.GetKey (KeyCode.Z)) {

			if (!zPressed) {
				//first time pressing Z, place cursor in center of canvas
				cursor.transform.localPosition = new Vector3((float)-((cursor.GetComponent<Renderer>()).bounds.size.x/2),
				                                             (float)0.1,
				                                             (float)((cursor.GetComponent<Renderer>()).bounds.size.y/2));
				cursor.GetComponent<Renderer>().enabled = true;
				zPressed = true;


			} else {
				//Z has been pressed for a while start moving the cursor
				moveCursor ();
			}

			//A BUTTON
			if (Input.GetKey (KeyCode.X)) {
				//start drawing

				Vector2 newPoint = cursorOnCanvas();

				if (pointIsOnCanvas(newPoint)) {
					drawPoint (newPoint);

					if (!xPressed) {
						lastpoint = new Vector2 (newPoint.x, newPoint.y);
						xPressed = true;
					}
					else {
						for (float i = 0.0f; i < 1.0f; i+=0.02f) {
							drawPoint(Vector2.Lerp(lastpoint, newPoint,i));
						}
						lastpoint = newPoint;
					}
					
					texture.Apply();
					empty = false;
				}
			}
		}
		if (!Input.GetKey (KeyCode.Z)) {
			//hide cursor
			zPressed = false;
			cursor.GetComponent<Renderer> ().enabled = false;
		}
		if (!Input.GetKey (KeyCode.X)) xPressed = false;
		
		if (Input.GetMouseButtonDown (0)) {
			isMousePressed = true;
		}
		
		if (Input.GetMouseButtonUp (0)) {
			isMousePressed = false;
			initial = true;
		}
		
		if (isMousePressed) {
			Ray camRay = thisCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit canvasHit;
			if (Physics.Raycast (camRay, out canvasHit, camRayLength, canvasMask)) 
			{
				Vector3 hitPoint = canvasHit.point;
				int x = (int) ((hitPoint.x - bottomLeft.position.x) * widthFactor);
				int y = (int) ((hitPoint.y - bottomLeft.position.y) * heightFactor);
				Vector2 newPoint = new Vector2 (x, y);
				drawPoint (newPoint);
				if (initial) {
					lastpoint = new Vector2 (newPoint.x, newPoint.y);
					initial = false;
				}
				else {
					for (float i = 0.0f; i < 1.0f; i+=0.02f) {
						drawPoint(Vector2.Lerp(lastpoint, newPoint,i));
					}
				}

				
				//while (lastpoint != newPoint) {
				//	drawPoint (lastpoint);
				//	lastpoint = Vector2.Lerp(lastpoint, newPoint, 0.5f);
				//}
				lastpoint = newPoint;

				texture.Apply();
				empty = false;
			}
			else {
				initial = true;
			}
		}
	}

	public void SaveImage() {
		//remove border
		for (int y = 0; y < texture.height; y++) {
			for (int x = 0; x < texture.width; x++) {
				if (x < 1 || y < 2 || x > texture.width - 2 || y > texture.height - 3)
					texture.SetPixel(x, y, Color.white);
			}
		}

		//save the image
		byte [] bytes = texture.EncodeToPNG ();
		if (!Directory.Exists("images")) Directory.CreateDirectory ("images");
		if (!Directory.Exists ("images/Output")) Directory.CreateDirectory("images/Output");
		File.WriteAllBytes ("images/Output/0.png", bytes);
	}
	
	public void clear () {
		for (int y = 0; y < texture.height; y++) {
			for (int x = 0; x < texture.width; x++) {
				if (x < 1 || y < 2 || x > texture.width - 2 || y > texture.height - 3)
					texture.SetPixel(x, y, Color.black);
				else
					texture.SetPixel(x, y, Color.white);
			}
		}
		texture.Apply();
		empty = true;
	}

	void OnEnable () {
		clear ();
	}

}


