/* Source: http://answers.unity3d.com/questions/184442/drawing-lines-from-mouse-position.html
 * Accessed: 10/4/15
 * 
 * Source: http://www.theappguruz.com/blog/draw-line-mouse-move-detect-line-collision-unity2d-unity3d
 * Accessed 10/3/15
 */

using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
	List<Vector3> linePoints = new List<Vector3>();
	LineRenderer lineRenderer;
	private bool isMousePressed;
	private float startWidth = 0.1f;
	private float endWidth = 0.1f;
	float threshold = 0.001f;
	Camera thisCamera;
	int lineCount = 0;
	int canvasMask;
	float camRayLength = 100f;
	
	Vector3 lastPos = Vector3.one * float.MaxValue;
	
	void Awake()
	{
		thisCamera = Camera.main;
		lineRenderer = gameObject.AddComponent<LineRenderer> ();
		lineRenderer.SetVertexCount (0);
		lineRenderer.material.color = Color.black;
		//lineRenderer.shadowCastingMode
		isMousePressed = true;
		canvasMask = LayerMask.GetMask ("Canvas");
	}
	
	void Update()
	{
		if (Input.GetMouseButtonUp (0)) {
			isMousePressed = false;
		}
		
		if (isMousePressed) {
			Ray camRay = thisCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit canvasHit;
			if (Physics.Raycast (camRay, out canvasHit, camRayLength, canvasMask)) 
			{
				Vector3 hitPoint = canvasHit.point;
				hitPoint.z-=0.01f;
				float dist = Vector3.Distance (lastPos, hitPoint);
				if (dist <= threshold)
					return;
				lastPos = hitPoint;
				if (linePoints == null)
					linePoints = new List<Vector3> ();
				linePoints.Add (hitPoint);
				UpdateLine ();
			}
		}
	}
	
	
	void UpdateLine()
	{
		lineRenderer.SetWidth(startWidth, endWidth);
		lineRenderer.SetVertexCount(linePoints.Count);
		
		for(int i = lineCount; i < linePoints.Count; i++)
		{
			lineRenderer.SetPosition(i, linePoints[i]);
		}
		lineCount = linePoints.Count;
	}
}