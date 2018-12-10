using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece {

	public override string toString(){return player.getSide().ToString() + " KNIGHT (" + piece_type_index + ")";}

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(1);
			else
				return GameManager.instance.GetCaseWithIndex(6);
		}else{ //black
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(57);
			else
				return GameManager.instance.GetCaseWithIndex(62);
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

	private List<Case> getUpLeftDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 7) // if we are on the LEFT bounds or TOP bounds
			return ret;

		int indexCorner = actualIndex + 7;
		GameManager gameManager = GameManager.instance;
		Case upCase, leftCase;
		if(indexCorner/8 != 7){ //if we are NOT on TOP bounds
			upCase = gameManager.GetCaseWithIndex(indexCorner + 8);
			if(upCase.isTaken()){ //if there's another piece on the case
				if(upCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					ret.Add(upCase);
				}else{// if it's an ally
					influencingCases.Add(upCase);
				}
			}
			else
				ret.Add(upCase);
		}
		if(indexCorner%8 != 0){ //if we are NOT on LEFT bounds
			leftCase = gameManager.GetCaseWithIndex(indexCorner -1);
			if(leftCase.isTaken()){ //if there's another piece on the case
				if(leftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					ret.Add(leftCase);
				}else{// if it's an ally
					influencingCases.Add(leftCase);
				}
			}
			else
				ret.Add(leftCase);
		}
		return ret;
	}

	private List<Case> getUpRightDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 7) // if we are on the RIGHT bounds or TOP bounds
			return ret;

		int indexCorner = actualIndex + 9;
		GameManager gameManager = GameManager.instance;
		Case upCase, rightCase;
		if(indexCorner/8 != 7){ //if we are NOT on TOP bounds
			upCase = gameManager.GetCaseWithIndex(indexCorner + 8);
			if(upCase.isTaken()){ //if there's another piece on the case
				if(upCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					ret.Add(upCase);
				}else{// if it's an ally
					influencingCases.Add(upCase);
				}
			}
			else{
				ret.Add(upCase);
			}
		}
		if(indexCorner%8 != 7){ //if we are NOT on RIGHT bounds
			rightCase = gameManager.GetCaseWithIndex(indexCorner +1);
			if(rightCase.isTaken()){ //if there's another piece on the case
				if(rightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					ret.Add(rightCase);
				}else{// if it's an ally
					influencingCases.Add(rightCase);
				}
			}
			else
				ret.Add(rightCase);
		}
		return ret;
	}

	private List<Case> getDownRightDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 0) // if we are on the RIGHT bounds or BOTTOM bounds
			return ret;

		int indexCorner = actualIndex - 7;
		GameManager gameManager = GameManager.instance;
		Case downCase, rightCase;
		if(indexCorner/8 != 0){ //if we are NOT on BOTTOM bounds
			downCase = gameManager.GetCaseWithIndex(indexCorner - 8);
			if(downCase.isTaken()){ //if there's another piece on the case
				if(downCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					ret.Add(downCase);
				}else{// if it's an ally
					influencingCases.Add(downCase);
				}
			}
			else
				ret.Add(downCase);
		}
		if(indexCorner%8 != 7){ //if we are NOT on RIGHT bounds
			rightCase = gameManager.GetCaseWithIndex(indexCorner +1);
			if(rightCase.isTaken()){ //if there's another piece on the case
				if(rightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					ret.Add(rightCase);
				}else{// if it's an ally
					influencingCases.Add(rightCase);
				}
			}
			else
				ret.Add(rightCase);
		}
		return ret;
	}

	private List<Case> getDownLeftDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 0) // if we are on the LEFT bounds or BOTTOM bounds
			return ret;

		int indexCorner = actualIndex - 9;
		GameManager gameManager = GameManager.instance;
		Case downCase, leftCase;
		if(indexCorner/8 != 0){ //if we are NOT on BOTTOM bounds
			downCase = gameManager.GetCaseWithIndex(indexCorner - 8);
			if(downCase.isTaken()){ //if there's another piece on the case
				if(downCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					ret.Add(downCase);
				}else{// if it's an ally
					influencingCases.Add(downCase);
				}
			}
			else
				ret.Add(downCase);
		}
		if(indexCorner%8 != 7){ //if we are NOT on RIGHT bounds
			leftCase = gameManager.GetCaseWithIndex(indexCorner -1);
			if(leftCase.isTaken()){ //if there's another piece on the case
				if(leftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					ret.Add(leftCase);
				}else{// if it's an ally
					influencingCases.Add(leftCase);
				}
			}
			else
				ret.Add(leftCase);
		}
		return ret;
	}
}
