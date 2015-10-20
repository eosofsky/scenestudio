using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
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

		texture = new Texture2D(500, 300);
		GetComponent<Renderer>().material.mainTexture = texture;
		
		clear ();

		lastpoint = new Vector2 (0,0);
		initial = true;

		widthFactor = (texture.width / (topRight.position.x - bottomLeft.position.x));
		heightFactor = (texture.height / (topRight.position.y - bottomLeft.position.y));
	}
	
	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			isMousePressed = true;
		}
		
		if (Input.GetMouseButtonUp (0)) {
			isMousePressed = false;
			initial = true;
		}
		
		if (isMousePressed) {
			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
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
		}
		
		if (Input.GetButton ("Jump")) {
			int size = texture.width * texture.height;
			Color[] array = texture.GetPixels ();
			float[] vals = new float[size * 4];
			for (int i = 0; i < size; i++) {
				Color pixel = array[i];
				vals[i*4] = pixel.r;
				vals[i*4+1] = pixel.g;
				vals[i*4+2] = pixel.b;
				vals[i*4+3] = pixel.a;
			}
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


