using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using System.IO;

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
	public String[] twoDim;
	public GameObject[] threeDim;
	private Dictionary<String, GameObject> modelMap;
	public float instantiateDistance = 5f;
	private Vector3 instantiateVector;
	
	//for Wiimote
	float calZ = -1;
	float calX = -100;
	float smooth = 20;
	int lastLineWiiMote;
	double oldAccX = 0, oldAccY = 0, oldAccZ = 0;
	double accX = 0, accY = 0, accZ = 0;
	public GameObject cursor;
	private bool zPressed;
	private bool xPressed;
	Quaternion rot; //for pointing to where cursor should go

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

	int getWiimoteCoords() {

		string coordinates = "wiimote.txt";
		string coordinateFile = Path.GetFileName (coordinates);
		string[] lines;
		string[][] values;
		
		//gets the coordinates from the file outputted by DarwiinRemote
		if (System.IO.File.Exists(coordinateFile)){ 
			string txt;
			
			using (StreamReader sr = new StreamReader(coordinateFile)) {
				//read two lines for the labels
				sr.ReadLine();
				sr.ReadLine();
				//get the updated information
				txt = sr.ReadToEnd();
				lines = txt.Split(new string[] { "\n" }, StringSplitOptions.None);
				values = new string[lines.Length][];
				for (int i = 0; i < lines.Length; i++) {
					values[i] = lines[i].Split(new string[] { "," }, StringSplitOptions.None);
				}
				
				double tempX = double.Parse(values[values.Length-2][1],CultureInfo.InvariantCulture);
				double tempY = double.Parse(values[values.Length-2][2],CultureInfo.InvariantCulture);
				double tempZ = double.Parse(values[values.Length-2][3],CultureInfo.InvariantCulture);
				
				if (tempX == oldAccX && tempY == oldAccY && tempZ == oldAccZ) {
					//no change, the wiimote probably disconnected
					accX = 0; accY = 0; accZ = 0;
				} else {
					//changed, so update to the new accelerations
					oldAccX = accX; oldAccY = accY; oldAccZ = accZ;
					accX = tempX; accY = tempY; accZ = tempZ - calZ;
				}

				/*Quaternion target = Quaternion.Euler ((float)(calZ-accZ*3), (float)0, (float)0);
				rot = Quaternion.Slerp (transform.rotation, target, Time.deltaTime*smooth);
				Debug.Log(rot);*/
			}
			
			//using (StreamWriter sw = new StreamWriter(coordinates, false)) {
			////refresh so that we won't read it in again next time
			//sw.WriteLine("time,AccX,AccY,AccZ,pressureTR, pressureBR, pressureTL,");
			//sw.WriteLine("pressureBL,rawPressureTR,rawPressureBR,rawPressureTL,rawPressureBL");
		}
		return 0;
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
		Vector3 oldPosition = cursor.transform.localPosition;
		cursor.transform.localPosition = new Vector3((float)(oldPosition.x+accX*0.5),
		                                             (float)(oldPosition.y),
		                                             (float)(oldPosition.z+((accZ)*0.5)));
	}
	
	void Awake ()
	{	
		isMousePressed = false;
		zPressed = false;
		
		canvasMask = LayerMask.GetMask ("Canvas");
		
		texture = new Texture2D(1000, 1000);
		GetComponent<Renderer>().material.mainTexture = texture;
		
		clear ();
		
		lastpoint = new Vector2 (0,0);
		initial = true;
		
		widthFactor = (texture.width / (topRight.position.x - bottomLeft.position.x));
		heightFactor = (texture.height / (topRight.position.y - bottomLeft.position.y));

		modelMap = new Dictionary<String, GameObject>();
		if (twoDim.Length != threeDim.Length) {
			print ("Error!");
		}
		for (int i = 0; i < twoDim.Length; i++) {
			modelMap.Add (twoDim [i], threeDim [i]);
		}

		instantiateVector = new Vector3 (0.0f, 0.0f, instantiateDistance);
	}
	
	/* 
	 * To use Wiimote, person should run DarwiinRemote and start recording into a file called
	 * "wiimote.txt" in the SceneStudio directory. Once that is up and running, run this file
	 * and the program will start reading coordinates from there.
	 */
	void Update()
	{	
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
				getWiimoteCoords ();
				moveCursor ();
			}
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
			}
			else {
				initial = true;
			}
		}
		
		if (Input.GetButton ("Cancel")) {
			//Remove border
			for (int y = 0; y < texture.height; y++) {
				for (int x = 0; x < texture.width; x++) {
					if (x < 5 || y < 8 || x > texture.width - 6 || y > texture.height - 9)
						texture.SetPixel(x, y, Color.white);
				}
			}
			byte [] bytes = texture.EncodeToJPG ();
			File.WriteAllBytes ("testout.jpg", bytes);
			
			//Run through ml algorithm here
			String output = "apple";
			//Vector3 loc = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye);
			//Quaternion rot = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye);
			//print (loc);
			//print (rot);
			//print (Quaternion.Euler(0.0f, rot.y, 0.0f) * instantiateVector);
			//Instantiate(modelMap[output], Quaternion.Euler(0.0f, rot.y * 100f, 0.0f) * instantiateVector, Quaternion.identity);
			Vector3 instantiateLoc = Camera.main.transform.position + Camera.main.transform.forward * instantiateDistance; //loc + Quaternion.Euler(0.0f, rot.y, 0.0f) * instantiateVector;
			//instantiateLoc.z = 5.0f;
			instantiateLoc.y = 0.0f;
			Instantiate(modelMap[output], instantiateLoc, Quaternion.identity);

//			int size = texture.width * texture.height;
//			Color[] array = texture.GetPixels ();
//			float[] vals = new float[size * 4];
//			for (int i = 0; i < size; i++) {
//				Color pixel = array[i];
//				vals[i*4] = pixel.r;
//				vals[i*4+1] = pixel.g;
//				vals[i*4+2] = pixel.b;
//				vals[i*4+3] = pixel.a;
//			}
			//print ("here");
		}
	}
	
	public void clear () {
		for (int y = 0; y < texture.height; y++) {
			for (int x = 0; x < texture.width; x++) {
				if (x < 5 || y < 8 || x > texture.width - 6 || y > texture.height - 9)
					texture.SetPixel(x, y, Color.black);
				else
					texture.SetPixel(x, y, Color.white);
			}
		}
		texture.Apply();
	}

	void OnEnable () {
		clear ();
	}

}


