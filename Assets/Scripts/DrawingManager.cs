using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
	private bool isMousePressed;
	int canvasMask;
	float camRayLength = 100f;
	Texture2D texture;
	public Transform bottomLeft;
	public Transform topRight;
	private float widthFactor;
	private float heightFactor;
	
	void Awake ()
	{	
		isMousePressed = false;
		
		canvasMask = LayerMask.GetMask ("Canvas");

		texture = new Texture2D(200, 200);
		GetComponent<Renderer>().material.mainTexture = texture;
		
		for (int y = 0; y < texture.height; y++) {
			for (int x = 0; x < texture.width; x++) {
				texture.SetPixel(x, y, Color.white);
			}
		}
		texture.Apply();

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
		}
		
		if (isMousePressed) {
			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit canvasHit;
			if (Physics.Raycast (camRay, out canvasHit, camRayLength, canvasMask)) 
			{
				Vector3 hitPoint = canvasHit.point;
				int x = (int) ((hitPoint.x - bottomLeft.position.x) * widthFactor);
				int y = (int) ((hitPoint.y - bottomLeft.position.y) * heightFactor);
				texture.SetPixel(x-1, y-1, Color.black);
				texture.SetPixel(x-1, y, Color.black);
				texture.SetPixel(x-1, y+1, Color.black);
				texture.SetPixel(x, y-1, Color.black);
				texture.SetPixel(x, y, Color.black);
				texture.SetPixel(x, y+1, Color.black);
				texture.SetPixel(x+1, y-1, Color.black);
				texture.SetPixel(x+1, y, Color.black);
				texture.SetPixel(x+1, y+1, Color.black);
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
	
	
}


