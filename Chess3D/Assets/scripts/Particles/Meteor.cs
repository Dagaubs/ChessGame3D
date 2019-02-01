using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {
	
	public float timeAnim = 1f, timePassed = 0;
	public GameObject hitGround_prefab;
	private Vector3 startPosition, targetPosition;
	private bool isInit = false;

	private Piece pieceToDestroy;
	public void Init(Piece attackedPiece)
	{
		pieceToDestroy = attackedPiece;
		startPosition = transform.position;
		targetPosition = new Vector3(startPosition.x, 0.5f, startPosition.z);
		isInit = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(isInit){
			timePassed += Time.deltaTime;
			transform.position = Vector3.Lerp(startPosition, targetPosition, timePassed / timeAnim);
			if(timePassed>= timeAnim){
				Instantiate(hitGround_prefab, targetPosition, Quaternion.Euler(90,0,0));
				pieceToDestroy.StartDeathAnim(false);
				Destroy(gameObject);
			}
		}
	}
}
