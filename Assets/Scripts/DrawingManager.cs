using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
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
	
	//for Wiimote
	int calY = 100;
	int calX = -100;
	int smooth = 20;
	int lastLineWiiMote;
	double oldAccX = 0, oldAccY = 0, oldAccZ = 0;
	double accX = 0, accY = 0, accZ = 0;
	
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
	
	void Awake ()
	{	
		isMousePressed = false;
		
		canvasMask = LayerMask.GetMask ("Canvas");

		texture = new Texture2D(1000, 1000);

		GetComponent<Renderer>().material.mainTexture = texture;
		
		clear ();
		
		lastpoint = new Vector2 (0,0);
		initial = true;
		
		widthFactor = (texture.width / (topRight.position.x - bottomLeft.position.x));
		heightFactor = (texture.height / (topRight.position.y - bottomLeft.position.y));
	}
	
	void Update()
	{
		/* 
		 * To use Wiimote, person should run DarwiinRemote and start recording into a file called
		 * "wiimote.txt" in the SceneStudio directory. Once that is up and running, run this file
		 * and the program will start reading coordinates from there.
		 */
		string coordinates = "wiimote.txt";
		string coordinateFile = System.IO.Path.GetFullPath (coordinates);
		
		string[] lines;
		string[][] values;
		
		UnityEngine.Debug.Log (coordinateFile);
		
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
					accX = tempX; accY = tempY; accZ = tempZ;
				}
				
				UnityEngine.Debug.Log (accX + " " + accY + " " + accZ);
			}
			
			//			using (StreamWriter sw = new StreamWriter(coordinates, false)) {
			//				//refresh so that we won't read it in again next time
			//				sw.WriteLine("time,AccX,AccY,AccZ,pressureTR, pressureBR, pressureTL,");
			//				sw.WriteLine("pressureBL,rawPressureTR,rawPressureBR,rawPressureTL,rawPressureBL");
			//			}
		}

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

				
				//				while (lastpoint != newPoint) {
//					drawPoint (lastpoint);
//					lastpoint = Vector2.Lerp(lastpoint, newPoint, 0.5f);
//				}
				lastpoint = newPoint;

				texture.Apply();
			}
			else {
				initial = true;
			}
		}
		
		if (Input.GetButton ("Cancel")) {
			byte [] bytes = texture.EncodeToJPG ();
			File.WriteAllBytes ("testout.jpg", bytes);
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
				texture.SetPixel(x, y, Color.white);
			}
		}
		texture.Apply();
	}

	void OnEnable () {
		clear ();
	}

}


