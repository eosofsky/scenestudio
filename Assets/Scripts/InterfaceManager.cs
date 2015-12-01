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
	private bool zPressed;
	private bool escPressed;
	private bool cPressed;
	private bool oPressed;
	private bool pPressed;
	private int lastEditMode;
	
	public GameObject normalState;  //no canvas
	public GameObject drawingState; //canvas up
	public GameObject pickState; //add model?
	public GameObject wrongState; //what model did you mean?
	public GameObject editState; //interact with models
	public GameObject systemState; //drawingsystem
	public Image predictedImage; //image that displays what was predicted
	public GameObject panel; //array of other objects
	public GameObject movement;
	private DrawingManager drawingManager;
	private Selection selector;
	private ScaleScript scaler;
	public List<GameObject> editButtons;
	
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
		editState.SetActive (false);
		systemState.SetActive (false);

		
		canvasOpen = false;
		
		predictor = GameObject.FindGameObjectWithTag ("Predictor").GetComponent<Predict> ();
		selector = GameObject.FindGameObjectWithTag ("ScaleSelection").GetComponent<Selection> ();
		scaler = GameObject.FindGameObjectWithTag ("ScaleSelection").GetComponent<ScaleScript> ();
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
		if (!Input.GetButton ("Submit")) submitPressed = false;
		if (!Input.GetKey (KeyCode.LeftArrow)) leftPressed = false;
		if (!Input.GetKey (KeyCode.RightArrow)) rightPressed = false;
		if (!Input.GetKey (KeyCode.Z)) zPressed = false;
		if (!Input.GetButton ("Cancel")) escPressed = false;
		if (!Input.GetKey (KeyCode.C)) cPressed = false;
		if (!Input.GetKey (KeyCode.O)) oPressed = false;
		if (!Input.GetKey (KeyCode.P)) pPressed = false;

		//in the normal state
		if (normalState.activeSelf) {
			movement.GetComponent<Move> ().SetActivated (true);
			//start drawing
			if (Input.GetButtonDown ("Submit") && !submitPressed) {
				submitPressed = true;
				if (editState.activeSelf) {
					selector.ToggleSelect();
					editButtons[lastEditMode].transform.localScale = new Vector3(1.0f,1.0f,1.0f);
				}
				draw ();
			}

			//toggle edit mode
			if (Input.GetKey (KeyCode.C) && !cPressed) {
				cPressed = true;
				bool select = selector.ToggleSelect();
				if (select) { 
					edit();
					editButtons[lastEditMode].transform.localScale = new Vector3(1.35f,1.35f,1.35f);
				} else {
					close();
					editButtons[lastEditMode].transform.localScale = new Vector3(1.0f,1.0f,1.0f);
				}
			}
		} else {
			movement.GetComponent<Move>().SetActivated(false);
		}

		//in edit mode
		if (editState.activeSelf) {
			//delete object
			if (Input.GetKey (KeyCode.Z) && !zPressed) {
				zPressed = true;
				scaler.Delete ();
			}

			//change modes
			if (Input.GetKey (KeyCode.O) && !oPressed) {
				oPressed = true;
				lastEditMode = scaler.ChangeMode(false);
				editButtons[(lastEditMode+1)%4].transform.localScale = new Vector3(1.0f,1.0f,1.0f);
				editButtons[lastEditMode].transform.localScale = new Vector3(1.35f,1.35f,1.35f);
			}
			if (Input.GetKey (KeyCode.P) && !pPressed) {
				pPressed = true;
				lastEditMode = scaler.ChangeMode(true);
				editButtons[(lastEditMode-1+4)%4].transform.localScale = new Vector3(1.0f,1.0f,1.0f);
				editButtons[lastEditMode].transform.localScale = new Vector3(1.35f,1.35f,1.35f);
			}
		}
		
		//in the drawing state
		if (drawingState.activeSelf) {
			//stop drawing
			if (Input.GetButtonDown ("Submit") && !submitPressed) {
				submitPressed = true;
				close ();
			}
			
			//submit drawing
			if (Input.GetButtonDown ("Cancel") && !drawingManager.empty && !escPressed) {
				escPressed = true;
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
			if (Input.GetKey (KeyCode.Z) && !zPressed) {
				wrongModel();
				zPressed = true;
			}
			
			//go back to drawing
			if (Input.GetButtonDown ("Cancel") && !escPressed) {
				escPressed = true;
				draw ();
				predictedImage.sprite = Resources.Load <Sprite>("default");
			}
			
			//go back to normal
			if (Input.GetButtonDown("Submit") && !submitPressed) {
				submitPressed = true;
				close ();
			}
		}
		
		//in the wrong state
		if (wrongState.activeSelf) {
			//switch selected objects
			if (Input.GetKey (KeyCode.LeftArrow) && !leftPressed) {
				leftPressed = true;
				ChangeSelection(false);
			}
			if (Input.GetKey (KeyCode.RightArrow) && !rightPressed) {
				rightPressed = true;
				ChangeSelection (true);
			}
			
			//select the object
			if (Input.GetKey (KeyCode.X)) {
				close();
				predictor.AddObject (mapping[selected]);
			}
			
			//ENTER - exit out of object placement mode (go back to normal)
			if (Input.GetButtonDown ("Submit")) {
				close ();
			}
			
			//B - go back to the object from before
			if (Input.GetKey (KeyCode.Z) && !zPressed) {
				doneDrawing ();
				zPressed = true;
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
		pos.x = 645f - (selected*200f*0.79201f);
		panel.GetComponent<RectTransform> ().anchoredPosition = pos;
	}
	
	//pull up canvas when draw button is clicked
	public void draw() {
		canvas.SetActive (true);
		canvasOpen = true;
		normalState.SetActive (false);
		editState.SetActive (false);
		pickState.SetActive (false);
		drawingState.SetActive (true);
		systemState.SetActive (true);
		editState.SetActive (false);
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
		editState.SetActive (false);
	}
	
	//either doesnt recognize, or player says model is wrong
	public void wrongModel() {
		wrongState.SetActive (true);
		ChangeSelection (true);
		ChangeSelection (false);
		drawingState.SetActive (false);
		pickState.SetActive (false);
		systemState.SetActive (true);
		editState.SetActive (false);
	}

	//edit state
	public void edit() {
		wrongState.SetActive (false);
		drawingState.SetActive (false);
		pickState.SetActive (false);
		systemState.SetActive (false);
		normalState.SetActive (true);
		editState.SetActive (true);
	}
	
	//sets back to normal state
	public void close() {
		editState.SetActive (false);
		canvas.SetActive (false);
		drawingState.SetActive (false);
		pickState.SetActive (false);
		wrongState.SetActive (false);
		systemState.SetActive (false);
		normalState.SetActive (true);
	}
}