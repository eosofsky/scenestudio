using UnityEngine;
using System.Collections;

public class Selection : MonoBehaviour {

	private bool selectingMode = false;
	private Camera thisCamera;
	private int selectableMask;
	public float rayDistance = 50.0f;
	public float rayRadius = 5.0f;
	private Vector3 screenCenter;
	private Vector3 deltaY;
	private Vector3 deltaX;
	public Light selectionLight;
	private ScaleScript scaleScript;

	void Start () {
		thisCamera = Camera.main;
		selectableMask = LayerMask.GetMask ("Selectable");
		screenCenter = new Vector3 (Screen.width / 2, Screen.height / 2);
		deltaY = new Vector3 (0.0f, rayRadius);
		deltaX = new Vector3 (rayRadius, 0.0f);
		scaleScript = gameObject.GetComponent <ScaleScript> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {
			selectingMode = !selectingMode;
		}
		if (selectingMode) {
			RaycastHit itemHit;
			GameObject target;
			if (Physics.Raycast (thisCamera.ScreenPointToRay (screenCenter), out itemHit, rayDistance, selectableMask)) 
			{
				target = itemHit.collider.gameObject;
				print("Hit center: " + target);
			}
			else if (Physics.Raycast (thisCamera.ScreenPointToRay (screenCenter + deltaY), out itemHit, rayDistance, selectableMask)) 
			{
				target = itemHit.collider.gameObject;
				print("Hit above: " + target);
			}
			else if (Physics.Raycast (thisCamera.ScreenPointToRay (screenCenter - deltaY), out itemHit, rayDistance, selectableMask)) 
			{
				target = itemHit.collider.gameObject;
				print("Hit below: " + target);
			}
			else if (Physics.Raycast (thisCamera.ScreenPointToRay (screenCenter + deltaX), out itemHit, rayDistance, selectableMask)) 
			{
				target = itemHit.collider.gameObject;
				print("Hit right: " + target);
			}
			else if (Physics.Raycast (thisCamera.ScreenPointToRay (screenCenter - deltaX), out itemHit, rayDistance, selectableMask)) 
			{
				target = itemHit.collider.gameObject;
				print("Hit left: " + target);
			}
			else {
				print ("Nope");
				selectionLight.enabled = false;
				return;
			}	
			selectionLight.transform.position = itemHit.point;
			selectionLight.enabled = true;
			print (selectionLight.transform.position);
			scaleScript.ChangeSelectedObject (target);
		}

	}
}
