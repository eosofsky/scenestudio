using UnityEngine;
using System.Collections;

public class InterfaceManager : MonoBehaviour {

	public GameObject canvas;
	private bool canvasOpen;
	
	// Use this for initialization
	void Start () {
		canvas.SetActive (false);
		canvasOpen = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Submit")) {
			if (canvasOpen) {
				canvas.SetActive(false);
				canvasOpen = false;
			}
			else {
				canvas.SetActive(true);
				canvasOpen = true;
			}
		}
	}
}
