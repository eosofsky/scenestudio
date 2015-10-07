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
	private float startWidth = 0.01f;
	private float endWidth = 0.01f;
	public float threshold = 0.001f;
	public Camera thisCamera;
	int lineCount = 0;
	
	Vector3 lastPos = Vector3.one * float.MaxValue;

	void Awake()
	{
		thisCamera = Camera.main;
		lineRenderer = gameObject.AddComponent<LineRenderer> ();
		lineRenderer.SetVertexCount (0);
		lineRenderer.material.color = Color.black;
		isMousePressed = true;
	}
	
	void Update()
	{
		if (Input.GetMouseButtonUp (0)) {
			isMousePressed = false;
		}

		if (isMousePressed) {
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = thisCamera.nearClipPlane;
			Vector3 mouseWorld = thisCamera.ScreenToWorldPoint (mousePos);
			
			float dist = Vector3.Distance (lastPos, mouseWorld);
			if (dist <= threshold)
				return;
			
			lastPos = mouseWorld;
			if (linePoints == null)
				linePoints = new List<Vector3> ();
			linePoints.Add (mouseWorld);
			
			UpdateLine ();
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