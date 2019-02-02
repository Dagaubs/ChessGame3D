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

	public static int Capital_A_Char_index = 65;

	[SerializeField]
	private case_type type;
	public case_type getType(){return type;}

	public string generalCoordinate = "";
	private float case_size;

	[SerializeField]
	private int indexInPlate;
	public int GetIndex(){return indexInPlate;}

	private Transform standingOnPieceTransform;
	public Transform GetStandingOnPieceTransform() { return standingOnPieceTransform;}

	[SerializeField]
	private bool accessibility = true, selected = false;
	public bool isAccessible(){ return accessibility;}
	public bool isSelected(){return selected;}
	public void setAccessibility(bool a){
		accessibility = a;
		if(accessibility){
			if(taken){
				accessibleTakenGo.SetActive(true);
				accessibleGo.SetActive(false);
			}
			else{
				accessibleTakenGo.SetActive(false);
				accessibleGo.SetActive(true);
			}
		}else{
			accessibleTakenGo.SetActive(false);
			accessibleGo.SetActive(false);
		}
	}

	public string toString(){
		return generalCoordinate + "(" + indexInPlate + ")";
	}

	[SerializeField]
	private bool taken = false;
	public bool isTaken(){return taken;}
	private Piece standingOnPiece = null;
	public Piece GetStandingOnPiece(){return standingOnPiece;}

	[SerializeField]
	private GameObject accessibleGo, accessibleTakenGo, selectedGo;

	public void LeavePiece(){
		taken = false;
		standingOnPiece = null;
	}

	public Vector3 GetAttackPosition(Piece attacker){
		Vector3 standingOnPiecePosition = standingOnPieceTransform.position;
		Vector3 a_Vector = standingOnPiecePosition, b_Vector = new Vector3(attacker.transform.position.x, 0.5f, attacker.transform.position.z);
		Vector3 dir = Vector3.Normalize(b_Vector - a_Vector);
		//Debug.Log("A : " + a_Vector + " | B : " + b_Vector + " | dir : " + dir);
		return standingOnPiecePosition + dir * case_size;
	}

	public Vector3 ComeOnPiece(Piece piece){
		//Debug.Log(toString() + " is taken by " + piece.toString());
		taken = true;
		standingOnPiece = piece;
		return standingOnPieceTransform.position;
	}

	public Vector3 ComeOnAttackPosition(Piece piece){
		taken = true;
		standingOnPiece = piece;
		return GetAttackPosition(piece);
	}

	public void PickPieceOnCase(){
		if(!taken){
			Debug.LogError("Can't SELECT Case because it's empty !");
			return;
		}
		selected = true;
		selectedGo.SetActive(selected);
		// 
	}

	public void UnpickPieceOnCase(){
		if(!selected){
			selectedGo.SetActive(false);
			Debug.LogError("Can't UNSELECT Case because it's not selected !");
			return;
		}
		selected = false;
		selectedGo.SetActive(selected);
	}

	public void init(int nb, float case_size = 1f){
		indexInPlate = nb;
		this.case_size = case_size;
		generalCoordinate = ((char)(Capital_A_Char_index + indexInPlate%8)).ToString() + (indexInPlate/8 + 1).ToString();
		transform.localScale = new Vector3(case_size, 1f, case_size);
		float firstindex = -4f * case_size + case_size / 2;
		transform.position = new Vector3(firstindex + (indexInPlate%8) * case_size, 0,firstindex + (indexInPlate/8) * case_size);
		gameObject.name = "Case_" + generalCoordinate + "(" + indexInPlate +")_" + type.ToString();
		standingOnPieceTransform = Instantiate(new GameObject("StandingOnPiece Transform"), transform, false).transform;
		standingOnPieceTransform.localPosition = Vector3.up * 0.5f;
	}

	public void WrongCaseChoiceAnimation(){
		StopAllCoroutines();
		StartCoroutine(WrongCaseChoiceCoroutine());
	}

	private IEnumerator WrongCaseChoiceCoroutine(){
		yield return null;
	}
}
