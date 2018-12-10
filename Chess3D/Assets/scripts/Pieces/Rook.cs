using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece {

	public override string toString(){return player.getSide().ToString() + " ROOK (" + piece_type_index + ")";}

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(0);
			else
				return GameManager.instance.GetCaseWithIndex(7);
		}else{ //black
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(56);
			else
				return GameManager.instance.GetCaseWithIndex(63);
		}
	}

	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
		List<Case> retUp = getUpVerticale(),
		retDown = getDownVerticale(),
		retLeft = getLeftHorizontale(),
		retRight = getRightHorizontale();
		if(retDown.Count>0)
			retUp.AddRange(retDown);

		if(retLeft.Count>0)
			retUp.AddRange(retLeft);

		if(retRight.Count>0)
			retUp.AddRange(retRight);
		accessibleCases = retUp;
	}

	private List<Case> getUpVerticale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 7) // if we are on the TOP bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+8; index < 64; index=index+8){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				Debug.Log(toString() + " : foundSomeone on : " + foundCase.toString());
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					Debug.Log(toString() + " : found an ally");
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
			if(index/8 == 7) // if we are on the top bounds
				return ret;
		}
		return ret;
	}

	private List<Case> getDownVerticale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 0) // if we are on the top bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-8; index > 0; index=index-8){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				Debug.Log(toString() + " : foundSomeone on : " + foundCase.toString());
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					Debug.Log(toString() + " : found an ally");
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
			if(index/8 == 0) // if we are on the top bounds
				return ret;
		}
		return ret;
	}

	private List<Case> getLeftHorizontale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0) // if we are on the LEFT bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-1; index > 0; index--){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				Debug.Log(toString() + " : foundSomeone on : " + foundCase.toString() + " | " + foundCase.GetStandingOnPiece().GetPlayer().toString() + " | " + (foundCase.GetStandingOnPiece().GetPlayer() == player).ToString());
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
			if(index%8 == 0) // if we are on the LEFT bounds
				return ret;
		}
		return ret;
	}

	private List<Case> getRightHorizontale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7) // if we are on the RIGHT bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+1; index < 64; index++){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				Debug.Log(toString() + " : foundSomeone on : " + foundCase.toString());
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
			if(index%8 == 7) // if we are on the RIGHT bounds
				return ret;
		}
		return ret;
	}
}
