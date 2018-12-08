using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour {

	public enum case_type{
		LIGHT,
		DARK
	}

	/*public Vector3 getPieceTransform(){
		return new Vector3(transform.position.x,transform.position.y + 0.1f, transform.position.z);
	}*/

	[SerializeField]
	private case_type type;
	public case_type getType(){return type;}

	private int indexInPlate;

	private bool accessibility = true;
	public void setAccessibility(bool a){accessibility = a;}
	public bool isAccessible(){ return accessibility;}

	private bool taken = false;
	public bool isTaken(){return taken;}
	private Piece standingOnPiece = null;
	public Piece getStandingOnPiece(){return standingOnPiece;}

	public void LeavePiece(){
		taken = false;
		standingOnPiece = null;
	}

	public Vector3 ComeOnPiece(Piece piece){
		taken = true;
		standingOnPiece = piece;
		return transform.position;
	}

	public void init(int nb){
		transform.position = new Vector3(-4 + (nb%8), 0,-4 + (nb/8));
	}

	public void WrongCaseChoiceAnimation(){
		StopAllCoroutines();
		StartCoroutine(WrongCaseChoiceCoroutine());
	}

	private IEnumerator WrongCaseChoiceCoroutine(){
		yield return null;
	}
}
