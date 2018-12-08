using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece {

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			return GameManager.instance.GetCaseWithIndex(3);
		}else{ //black
			return GameManager.instance.GetCaseWithIndex(59);
		}
	}

	protected override void LookForAccessibleCases(){
		List<Case> ret = getUpLeftDiagonale();
		ret.AddRange(getUpRightDiagonale());
		ret.AddRange(getDownRightDiagonale());
		ret.AddRange(getDownLeftDiagonale());
		ret.AddRange(getUpVerticale());
		ret.AddRange(getDownVerticale());
		ret.AddRange(getLeftHorizontale());
		ret.AddRange(getRightHorizontale());
		accessibleCases = ret;
	}

	private List<Case> getUpVerticale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 7) // if we are on the top bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+8; index < 64; index=index+8){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			ret.Add(foundCase);
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
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			ret.Add(foundCase);
			if(index/8 == 0) // if we are on the top bounds
				return ret;
		}
		return ret;
	}

	private List<Case> getLeftHorizontale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0) // if we are on the top bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-1; index > 0; index--){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			ret.Add(foundCase);
			if(index%8 == 0) // if we are on the top bounds
				return ret;
		}
		return ret;
	}

	private List<Case> getRightHorizontale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 70) // if we are on the right bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+1; index > 0; index++){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			ret.Add(foundCase);
			if(index%8 == 7) // if we are on the top bounds
				return ret;
		}
		return ret;
	}

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
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			ret.Add(foundCase);
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
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			ret.Add(foundCase);
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
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			ret.Add(foundCase);
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
					return ret;
				}else{
					ret.Add(foundCase);
					return ret;
				}
			}
			ret.Add(foundCase);
			if(index%8 == 0 || index/8 == 0) // if we are on the LEFT bounds OR BOTTOM bounds
				return ret;
		}
		return ret;
	}
}
