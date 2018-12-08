using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {

	protected Case actualCase;

	protected int piece_type_index = 1;
	public void setIndex(int i){piece_type_index = i;}
	public int getIndex(){return piece_type_index;}

	protected Player player;
	public virtual Player GetPlayer(){return player;}

	protected List<Case> accessibleCases = null;
	protected virtual List<Case> GetAccessibleCases(){
		LookForAccessibleCases();
		return accessibleCases;
	}

	private bool dead = false;
	public bool isDead(){return dead;}

	public virtual void Init(Player p){
		player = p;
		GoTo(getInitialCase());
		if(p.getSide() == Player.PlayerSide.BLACK){
			transform.localEulerAngles = Vector3.up * 180f;
		}
		Material m = player.getSide() == Player.PlayerSide.WHITE ? GameManager.instance.whiteMaterial : GameManager.instance.blackMaterial;
		transform.GetChild(0).GetComponent<Renderer>().material = m;
	}

	public virtual void Pick(){
		Debug.Log("Pick is call on " + actualCase.GetIndex());
		actualCase.PickPieceOnCase();
		List<Case> casesTosetAccessible = GetAccessibleCases();
		foreach(Case c in casesTosetAccessible)
			c.setAccessibility(true);
	}

	public virtual void Unpick(){
		actualCase.UnpickPieceOnCase();
		List<Case> casesTosetAccessible = GetAccessibleCases();
		foreach(Case c in casesTosetAccessible)
			c.setAccessibility(false);
	}

	public virtual void UnpickExceptTarget(Case targetCase){
		actualCase.UnpickPieceOnCase();
		List<Case> casesTosetAccessible = GetAccessibleCases();
		foreach(Case c in casesTosetAccessible){
			if(c != targetCase)
				c.setAccessibility(false);
		}
	}

	protected abstract void LookForAccessibleCases();

	protected abstract Case getInitialCase();

	public virtual void GoTo(Case targetCase){
		if(targetCase.isAccessible()){ // if it is legit to go to this case
			if(targetCase.isTaken()){ // if there's already a piece on this target
				Piece foundPiece = targetCase.GetStandingOnPiece();
				if(foundPiece.GetPlayer() != player){ // if it's an enemy piece
					foundPiece.GetEaten();
				}else{
					Debug.LogError("Found an ally piece ! this case shouldn't be accessible !");
					targetCase.WrongCaseChoiceAnimation();
					return;
				}
			}
			if(actualCase != null)
				actualCase.LeavePiece();
			targetCase.setAccessibility(false);
			transform.localPosition = transform.parent.InverseTransformPoint(targetCase.ComeOnPiece(this));
			actualCase = targetCase;
			LookForAccessibleCases();
		}
	}

	public virtual void GetEaten(){
		// leave piece
		actualCase.LeavePiece();
		dead = true;
		// inform player and GameManager
		player.LoseThisPiece(this);
	}

}
