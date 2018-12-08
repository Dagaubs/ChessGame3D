using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {

	protected Case actualCase;

	protected int piece_type_index = 1;
	public void setIndex(int i){piece_type_index = i;}
	public int getIndex(){return piece_type_index;}

	protected Player player;
	public virtual Player getPlayer(){return player;}

	public virtual void Init(Player p){
		player = p;
		GoTo(getInitialCase());
		Material m = player.getSide() == Player.PlayerSide.WHITE ? GameManager.instance.whiteMaterial : GameManager.instance.blackMaterial;
		transform.GetChild(0).GetComponent<Renderer>().material = m;
	}

	

	protected abstract Case getInitialCase();

	public virtual void GoTo(Case targetCase){
		if(targetCase.isAccessible()){ // if it is legit to go to this case
			if(targetCase.isTaken()){ // if there's already a piece on this target
				Piece foundPiece = targetCase.getStandingOnPiece();
				if(foundPiece.getPlayer() != player){ // if it's an ennemy piece
					foundPiece.GetEaten();
				}else{
					Debug.LogError("Found an ally piece ! this case shouldn't be accessible !");
					targetCase.WrongCaseChoiceAnimation();
					return;
				}
			}

			transform.localPosition = transform.parent.InverseTransformPoint(targetCase.ComeOnPiece(this));
			actualCase = targetCase;
		}
	}

	public virtual void GetEaten(){
		// leave piece
		// inform player and GameManager
		// destroy
	}

}
