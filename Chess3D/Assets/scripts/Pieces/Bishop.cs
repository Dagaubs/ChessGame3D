using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece {

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(2);
			else
				return GameManager.instance.GetCaseWithIndex(5);
		}else{ //black
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(58);
			else
				return GameManager.instance.GetCaseWithIndex(61);
		}
	}
	
	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
		List<Case> ret = getUpLeftDiagonale();
		ret.AddRange(getUpRightDiagonale());
		ret.AddRange(getDownRightDiagonale());
		ret.AddRange(getDownLeftDiagonale());
		accessibleCases = ret;
	}

	public override string toString(){return player.getSide().ToString() + " BISHOP (" + piece_type_index + ")";}

	private List<Case> getUpLeftDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 7) // if we are on the LEFT bounds OR TOP bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+7; index < 64; index=index+7){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					Debug.Log(toString() + " FOUND AN ALLY!");
					influencingCases.Add(foundCase);
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			else{
				ret.Add(foundCase);
			}
			if(index%8 == 0 || index/8 == 7) // if we are on the LEFT bounds OR TOP bounds
				return ret;
		}
		return ret;
	}

	private List<Case> getUpRightDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 7) // if we are on the RIGHT bounds OR TOP bounds
				return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+9; index < 64; index=index+9){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			else{
				ret.Add(foundCase);
			}
			if(index%8 == 7 || index/8 == 7) // if we are on the RIGHT bounds OR TOP bounds
				return ret;
		}
		return ret;
	}

	private List<Case> getDownRightDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 0) // if we are on the RIGHT bounds OR BOTTOM bounds
				return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-7; index > 0; index=index-7){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			else{
				ret.Add(foundCase);
			}
			if(index%8 == 7 || index/8 == 0) // if we are on the RIGHT bounds OR BOTTOM bounds
				return ret;
		}
		return ret;
	}

	private List<Case> getDownLeftDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 0) // if we are on the LEFT bounds OR BOTTOM bounds
				return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-9; index > 0; index=index-9){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			else{
				ret.Add(foundCase);
			}
			if(index%8 == 0 || index/8 == 0) // if we are on the LEFT bounds OR BOTTOM bounds
				return ret;
		}
		return ret;
	}

}
