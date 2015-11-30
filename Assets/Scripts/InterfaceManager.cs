using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {

	public GameObject canvas;
	private bool canvasOpen;
	private bool submitPressed;
	private bool leftPressed;
	private bool rightPressed;

	public GameObject normalState;  //no canvas
	public GameObject drawingState; //canvas up
	public GameObject pickState; //add model?
	public GameObject wrongState; //what model did you mean?
	public GameObject systemState; //drawingsystem
	public Image predictedImage; //image that displays what was predicted
	public GameObject panel; //array of other objects
	public GameObject leftArrow;
	public GameObject movement;
	public List<GameObject> panelObjects;
	public DrawingManager drawingManager;

	private Predict predictor;
	private int[] mapping;

	private int selected; //for the pick state
	
	// Use this for initialization
	void Start () {
		canvas.SetActive (false);
		normalState.SetActive (true);
		drawingState.SetActive (false);
		pickState.SetActive (false);
		wrongState.SetActive (false);
		systemState.SetActive (false);
		
		canvasOpen = false;

		predictor = GameObject.FindGameObjectWithTag ("Predictor").GetComponent<Predict> ();

		mapping = new int[10];
		mapping [0] = 4;
		mapping [1] = 6;
		mapping [2] = 5;
		mapping [3] = 8;
		mapping [4] = 10;
		mapping [5] = 1;
		mapping [6] = 2;
		mapping [7] = 3;
		mapping [8] = 7;
		mapping [9] = 9;
	}

	public void Toggle () {
		if (canvasOpen) {
			canvas.SetActive(false);
			canvasOpen = false;
		}
		else {
			canvas.SetActive(true);
			canvasOpen = true;
		}
	}

	// Update is called once per frame
	void Update () {
		//reset inputs
		if (!Input.GetButton ("Submit")) {
			submitPressed = false;
		}
		if (!Input.GetKey (KeyCode.LeftArrow)) {
			leftPressed = false;
		}
		if (!Input.GetKey (KeyCode.RightArrow)) {
			rightPressed = false;
		}

		//in the normal state
		if (normalState.activeSelf) {
			movement.GetComponent<Move> ().SetActivated (true);
			//start drawing
			if (Input.GetButtonDown ("Submit") && !submitPressed) {
				submitPressed = true;
				draw ();
			}
		} else {
			movement.GetComponent<Move>().SetActivated(false);
		}

		//in the drawing state
		if (drawingState.activeSelf) {
			//stop drawing
			if (Input.GetButtonDown ("Submit") && !submitPressed) {
				submitPressed = true;
				close ();
			}

			//submit drawing
			if (Input.GetButtonDown ("Cancel") && !drawingManager.empty) {
				doneDrawing ();
				canvas.GetComponent<DrawingManager>().SaveImage();
				predictor.RunArt();
			}
		}

		//in the pick state
		if (pickState.activeSelf) {
			//correct model - A Button
			if (Input.GetKey (KeyCode.X)) {
				close ();
				predictor.add ();
				predictedImage.sprite = Resources.Load <Sprite>("default");
			}

			//wrong model - B Button
			if (Input.GetKey (KeyCode.Z)) {
				wrongModel();
			}

			//go back to drawing
			if (Input.GetButtonDown ("Submit") && !submitPressed) {
				submitPressed = true;
				draw ();
				predictedImage.sprite = Resources.Load <Sprite>("default");
			}
		}

		//in the wrong state
		if (wrongState.activeSelf) {
			if (Input.GetKey (KeyCode.LeftArrow) && !leftPressed) {
				leftPressed = true;
				ChangeSelection(false);
			}

			if (Input.GetKey (KeyCode.RightArrow) && !rightPressed) {
				rightPressed = true;
				ChangeSelection (true);
			}

			if (Input.GetKey (KeyCode.X)) {
				close();
				predictor.AddObject (mapping[selected]);
			}
		}

	}

	private void ChangeSelection(bool right) {
		if (right) {
			selected = (selected + 1) % 10;
		} else {
			selected = (selected - 1 + 10) % 10;
		}
		Debug.Log ("selected is " + selected);

		RectTransform rect = panel.GetComponent<RectTransform> ();
		Vector3 pos = rect.anchoredPosition;
		pos.x = 495f - (selected*200f*0.79201f);
		panel.GetComponent<RectTransform> ().anchoredPosition = pos;
	}

	//pull up canvas when draw button is clicked
	public void draw() {
		canvas.SetActive (true);
		canvasOpen = true;
		normalState.SetActive (false);
		pickState.SetActive (false);
		drawingState.SetActive (true);
		systemState.SetActive (true);
		drawingManager = GameObject.FindGameObjectWithTag ("Drawing Manager").GetComponent<DrawingManager> ();
	}
	
	//call algorithm, open add state
	public void doneDrawing() {
		//loading state?
		wrongState.SetActive (false);
		pickState.SetActive (true);
		drawingState.SetActive (false);
		canvas.SetActive (false);
		systemState.SetActive (true);
	}

	//either doesnt recognize, or player says model is wrong
	public void wrongModel() {
		wrongState.SetActive (true);
		drawingState.SetActive (false);
		pickState.SetActive (false);
		systemState.SetActive (true);
	}
	
	//sets back to normal state
	public void close() {
		canvas.SetActive (false);
		normalState.SetActive (true);
		drawingState.SetActive (false);
		pickState.SetActive (false);
		wrongState.SetActive (false);
		systemState.SetActive (false);
	}
}