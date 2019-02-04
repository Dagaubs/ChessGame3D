using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFPS : MonoBehaviour {

	public float speed;
	public float sensitivity;

	private Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow)||Input.GetKey(KeyCode.Z)){
			transform.position += Vector3.up *speed;
			Debug.LogWarning("up");
		}
		if(Input.GetKey(KeyCode.DownArrow)||Input.GetKey(KeyCode.S)){
			transform.position -= Vector3.up *speed;
			Debug.LogWarning("down");
		}
		if(Input.GetKey(KeyCode.LeftArrow)||Input.GetKey(KeyCode.Q)){
			transform.position -= Vector3.right *speed;
			Debug.LogWarning("left");
		}
		if(Input.GetKey(KeyCode.RightArrow)||Input.GetKey(KeyCode.D)){
			transform.position += Vector3.right *speed;
			Debug.LogWarning("right");
		}

	 	Vector3 vp = cam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
 		vp.x -= 0.5f; vp.y -= 0.5f;
 		vp.x *= sensitivity; vp.y *= sensitivity;
 		vp.x += 0.5f; vp.y += 0.5f;
 		Vector3 sp = cam.ViewportToScreenPoint(vp);
 
 		Vector3 v = cam.ScreenToWorldPoint(sp);
 		transform.LookAt(v, Vector3.up);
	}
}
