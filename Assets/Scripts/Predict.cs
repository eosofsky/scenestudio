using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Globalization;
using UnityEngine.UI;
using System.Collections.Generic;

public class Predict : MonoBehaviour {
	
	public GameObject[] objects;
	public List<GameObject> objectsInScene;
	private string[] classifications = new string[11];
	public bool predicted;
	public float instantiateDistance = 5f;
	public Text txtRef;
	public Image predictImage;
	public bool addModel;


	public void RunArt () {
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo.UseShellExecute = true;
		process.StartInfo.FileName = "/bin/bash";
		process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory ();
		process.StartInfo.Arguments = "-c \"./art images/Output 0\"";
		process.Start ();
	}

	int[] Prediction (string prediction) {
		int[] values = new int[3];
		using (StreamReader sr = new StreamReader(prediction)) {
			
			string[] txt = File.ReadAllLines(prediction);
			
			//get the top 3 classifications
			for (int i = 0; i < 3; i++) {
				values[i] = int.Parse(txt[i],CultureInfo.InvariantCulture);
			}
		}
		
		return values;
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
		addModel = false;
	}


	// Update is called once per frame
	public void Update () {
		string prediction = Path.GetFileName ("prediction.txt");

		if (System.IO.File.Exists (prediction)) {
			//gets the predictions
			int[] predict = Prediction (prediction);
			string classification = classifications[predict[0]];
			Debug.Log ("Predicted to be " + classification);

			//add predicted object to scene?
			txtRef.text = ("add " + classification + " to scene?");
			string spriteLoc = classification + "_2";
			Sprite predSprite =  Resources.Load <Sprite>(spriteLoc);
			if (predSprite){
				predictImage.sprite = predSprite;
			} else {
				Debug.LogError("Sprite not found" + spriteLoc, this);
			}

			//if correct, add
			if (addModel) {
				AddObject(predict[0], prediction);
			} else {
				//TODO: allow them to select the correct one
				//Debug.Log("said no", this);
			} 

			//make sure the prediction only runs once
			if (System.IO.File.Exists ("blept.txt")) File.Delete ("blept.txt");
			File.Move (prediction,"blept.txt");
		}
	}
	public void add()
	{
		addModel = true;
	}
	//add corrected object

	public void newObject(int chosen)
	{
		GameObject obj = objects[chosen];
		Vector3 instantiateLoc = Camera.main.transform.position + Camera.main.transform.forward * instantiateDistance;
		instantiateLoc.y = obj.transform.position.y;
		Instantiate(obj, instantiateLoc, obj.transform.rotation);
		//Debug.Log ("newobj", this);
	}
	
	public void AddObject(int predict, string prediction) {
		//place the object into the scene
		GameObject obj = objects[predict];
		objectsInScene.Add (obj);
		Vector3 instantiateLoc = Camera.main.transform.position + Camera.main.transform.forward * instantiateDistance;
		instantiateLoc.y = obj.transform.position.y;
		Instantiate(obj, instantiateLoc, obj.transform.rotation);

		addModel = false;

	}
}