using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Globalization;
using UnityEngine.UI;

public class Predict : MonoBehaviour {
	
	public GameObject[] objects;
	private string[] classifications = new string[11];
	public bool predicted;
	public float instantiateDistance = 5f;
	public Text txtRef;
	public Image predictImage;


	public void RunArt () {
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo.UseShellExecute = true;
		process.StartInfo.FileName = "/bin/bash";
		process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory ();
		process.StartInfo.Arguments = "-c \"./art images/Output 0\"";
		process.Start ();
	}

	int Prediction (string prediction) {
		int final = 0;

		using (StreamReader sr = new StreamReader(prediction)) {
			string[] txt = File.ReadAllLines(prediction);
			int[] values = new int[11];

			for (int i = 0; i < values.Length; i++) {
				values[i] = 0;
			}

			//get all predictions
			for (int i = 0; i < txt.Length/2; i++) {
				int p = int.Parse(txt[i],CultureInfo.InvariantCulture);
				values[p]++;
			}

			//get mode
			int max = 0;
			for (int i = 0; i < values.Length; i++) {
				if (values[i] > max) {
					final = i;
					max = values[i];
				}
			}
		}

		return final;
	}

	// Use this for initialization
	void Start () {
		//initialize array
		classifications [1] = "Airplane";
		classifications [2] = "Bike";
		classifications [3] = "Fish";
		classifications [4] = "Lightbulb";
		classifications [5] = "Monkey";
		classifications [6] = "Pig";
		classifications [7] = "Pizza";
		classifications [8] = "Snowman";
		classifications [9] = "Tree";
		classifications [10] = "Apple";

		predicted = true;
	}
	
	// Update is called once per frame
	public void Update () {
		if (!predicted) {
			RunArt ();
			predicted = true;
		} else {
			string prediction = Path.GetFileName ("prediction.txt");
			//gets the coordinates from the file outputted by DarwiinRemote
			if (System.IO.File.Exists (prediction)) {
				int predict = Prediction (prediction);
				string classification = classifications[predict];
				Debug.Log ("Predicted to be " + classification);
				//check




				txtRef.text = ("add " + classification + " to scene?");

				//Sprite newSprite =  Resources.Load <Sprite>(classification + "_2");
				string spriteLoc = classification + "_2";
				Sprite predSprite =  Resources.Load <Sprite>(spriteLoc);
				if (predSprite){
					predictImage.sprite = predSprite;
				} else {
					Debug.LogError("Sprite not found" + spriteLoc, this);
				}
				//if 
				//predictImage.sprite = Resources.Load (classification + "2") as Sprite;
				//TODO: call other script to place the object or something
				GameObject obj = objects[predict];
				Vector3 instantiateLoc = Camera.main.transform.position + Camera.main.transform.forward * instantiateDistance;
				instantiateLoc.y = obj.transform.position.y;
				Instantiate(obj, instantiateLoc, obj.transform.rotation);//, Quaternion.identity);

				if (System.IO.File.Exists ("blept.txt")) File.Delete ("blept.txt");
				File.Move (prediction,"blept.txt");
			}
		}
	}
}