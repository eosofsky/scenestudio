using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Source: http://www.theappguruz.com/blog/draw-line-mouse-move-detect-line-collision-unity2d-unity3d
 * Accessed 10/3/15
 */

public class DrawLine : MonoBehaviour
{
	private LineRenderer line;
	private bool isMousePressed;
	private Vector3 mousePos;
	private int numVertices;

	void Awake ()
	{
		numVertices = 0;
		// Create line renderer component and set its property
		line = gameObject.AddComponent<LineRenderer> ();
		line.material = new Material (Shader.Find ("Standard"));
		line.SetVertexCount (0);
		line.SetWidth (1f, 1f);
		line.SetColors (Color.black, Color.black);
		line.useWorldSpace = true;	
		isMousePressed = false;
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			isMousePressed = true;
		}
		
		if (Input.GetMouseButtonUp (0)) {
			isMousePressed = false;
		}

		// Drawing line when mouse is moving(presses)
		if (isMousePressed) {
			mousePos = Input.mousePosition; //Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePos.x = mousePos.x / Screen.width;
			mousePos.y = mousePos.y / Screen.height;
			//mousePos.z = 750;
			print (mousePos);
			line.SetVertexCount (numVertices + 1);
			line.SetPosition (numVertices, mousePos);
			numVertices++;
		}
	}
}


