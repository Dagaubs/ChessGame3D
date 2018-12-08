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

	[SerializeField]
	private int indexInPlate;
	public int GetIndex(){return indexInPlate;}

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

	public Vector3 ComeOnPiece(Piece piece){
		taken = true;
		standingOnPiece = piece;
		return transform.position;
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

	public void init(int nb){
		indexInPlate = nb;
		transform.position = new Vector3(-4 + (indexInPlate%8), 0,-4 + (indexInPlate/8));
	}

	public void WrongCaseChoiceAnimation(){
		StopAllCoroutines();
		StartCoroutine(WrongCaseChoiceCoroutine());
	}

	private IEnumerator WrongCaseChoiceCoroutine(){
		yield return null;
	}
}
