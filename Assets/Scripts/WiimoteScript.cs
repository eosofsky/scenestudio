using UnityEngine;
using System.Collections;
using System.IO;
using System.Globalization;
using System;

public class WiimoteScript : MonoBehaviour {
	private float calZ = -1;
	private int lastLineWiiMote;
	public float accX = 0, accY = 0, accZ = 0;
	private InterfaceManager interfaceManager;

	public void getWiimoteCoords() {
		string coordinates = "wiimote.txt";
		string coordinateFile = Path.GetFileName (coordinates);
		string[] values;
		
		//gets the coordinates from the file outputted by DarwiinRemote
		if (System.IO.File.Exists(coordinateFile)){ 
			
			using (StreamReader sr = new StreamReader(coordinateFile)) {
				//read two lines for the labels
				sr.ReadLine();
				sr.ReadLine();
				//get the updated information
				string[] txt = File.ReadAllLines(coordinateFile);
				values = txt[txt.Length-2].Split (new[] { "," }, StringSplitOptions.None);
				
				float tempX = float.Parse(values[1],CultureInfo.InvariantCulture);
				float tempY = float.Parse(values[2],CultureInfo.InvariantCulture);
				float tempZ = float.Parse(values[3],CultureInfo.InvariantCulture);
				
				if (Math.Abs (accX - tempX) > 0.1) {
					accX = tempX;
				}
				if (Math.Abs (accY - tempY) > 0.1) {
					accY = tempY;
				}
				if (Math.Abs (accZ - tempZ) > 0.1) {
					accZ = tempZ-calZ;
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		interfaceManager = GameObject.FindGameObjectWithTag ("Interface Manager").GetComponent<InterfaceManager>();
	}
	
	// Update is called once per frame
	void Update () {
		getWiimoteCoords ();
	}
}
