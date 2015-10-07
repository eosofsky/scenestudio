using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
	public GameObject draw;

	void Update()
	{
		if (Input.GetMouseButtonDown (0))
			Instantiate (draw);

		if (Input.GetButton ("Jump")) 
			Application.CaptureScreenshot ("drawingTest");
	}
	
}

