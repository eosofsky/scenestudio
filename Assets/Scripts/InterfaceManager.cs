using UnityEngine;
using System.Collections;

public class InterfaceManager : MonoBehaviour {

	public GameObject canvas;
	private bool canvasOpen;

	public GameObject normalState;  //no canvas
	public GameObject drawingState; //canvas up
	public GameObject pickState; //add model?
	public GameObject wrongState; //what model did you mean?
	public GameObject addState; //add selected model??
	public GameObject systemState; //drawingsystem
	
	
	// Use this for initialization
	void Start () {
		canvas.SetActive (false);
		normalState.SetActive (true);
		drawingState.SetActive (false);
		pickState.SetActive (false);
		wrongState.SetActive (false);
		addState.SetActive (false);
		systemState.SetActive (false);
		
		canvasOpen = false;
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
		if (Input.GetButtonDown ("Submit")) {
			Toggle ();
		}
	}
	//pull up canvas when draw button is clicked
	public void draw() {
		canvas.SetActive (true);
		canvasOpen = true;
		normalState.SetActive (false);
		pickState.SetActive (false);
		drawingState.SetActive (true);
		systemState.SetActive (true);
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
		addState.SetActive (false);
		systemState.SetActive (true);
	}

	//confirm new model selection
	public void newModel() {
		addState.SetActive (true);
		wrongState.SetActive (false);
		systemState.SetActive (true);
	}
	
	//sets back to normal state
	public void close() {
		canvas.SetActive (false);
		normalState.SetActive (true);
		drawingState.SetActive (false);
		pickState.SetActive (false);
		wrongState.SetActive (false);
		addState.SetActive (false);
		systemState.SetActive (false);
	}
}