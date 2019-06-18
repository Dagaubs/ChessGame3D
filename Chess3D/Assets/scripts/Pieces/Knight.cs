using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece {

    public override PieceType getType()
    {
        return PieceType.KNIGHT;
    }
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

	public override bool CheckForCheck(){
		bool ret = false;
		ret = ret || checkKnightUpLeftDiagonale();
		ret = ret || checkKnightUpRightDiagonale();
		ret = ret || checkKnightDownRightDiagonale();
		return ret || checkKnightDownLeftDiagonale();
	}

	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
		List<Case> ret = getKnightUpLeftDiagonale();
		ret.AddRange(getKnightUpRightDiagonale());
		ret.AddRange(getKnightDownRightDiagonale());
		ret.AddRange(getKnightDownLeftDiagonale());
		accessibleCases = ret;
	}

	private List<Case> getKnightUpLeftDiagonale(){
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
					if(AddIfPotentialMove(upCase)){
						ret.Add(upCase);
					}
				}else{// if it's an ally
					influencingCases.Add(upCase);
				}
			}
			else if(AddIfPotentialMove(upCase)){
				ret.Add(upCase);
			}
		}
		if(indexCorner%8 != 0){ //if we are NOT on LEFT bounds
			leftCase = gameManager.GetCaseWithIndex(indexCorner -1);
			if(leftCase.isTaken()){ //if there's another piece on the case
				if(leftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(AddIfPotentialMove(leftCase)){
						ret.Add(leftCase);
					}
				}else{// if it's an ally
					influencingCases.Add(leftCase);
				}
			}
			else if(AddIfPotentialMove(leftCase)){
				ret.Add(leftCase);
			}
		}
		return ret;
	}

	private List<Case> getKnightUpRightDiagonale(){
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
					if(AddIfPotentialMove(upCase)){
						ret.Add(upCase);
					}
				}else{// if it's an ally
					influencingCases.Add(upCase);
				}
			}
			else if(AddIfPotentialMove(upCase)){
				ret.Add(upCase);
			}
		}
		if(indexCorner%8 != 7){ //if we are NOT on RIGHT bounds
			rightCase = gameManager.GetCaseWithIndex(indexCorner +1);
			if(rightCase.isTaken()){ //if there's another piece on the case
				if(rightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(AddIfPotentialMove(rightCase)){
						ret.Add(rightCase);
					}
				}else{// if it's an ally
					influencingCases.Add(rightCase);
				}
			}
			else if(AddIfPotentialMove(rightCase)){
				ret.Add(rightCase);
			}
		}
		return ret;
	}

	private List<Case> getKnightDownRightDiagonale(){
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
					if(AddIfPotentialMove(downCase)){
						ret.Add(downCase);
					}
				}else{// if it's an ally
					influencingCases.Add(downCase);
				}
			}
			else if(AddIfPotentialMove(downCase)){
				ret.Add(downCase);
			}
		}
		if(indexCorner%8 != 7){ //if we are NOT on RIGHT bounds
			rightCase = gameManager.GetCaseWithIndex(indexCorner +1);
			if(rightCase.isTaken()){ //if there's another piece on the case
				if(rightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(AddIfPotentialMove(rightCase)){
						ret.Add(rightCase);
					}
				}else{// if it's an ally
					influencingCases.Add(rightCase);
				}
			}
			else if(AddIfPotentialMove(rightCase)){
				ret.Add(rightCase);
			}
		}
		return ret;
	}

	private List<Case> getKnightDownLeftDiagonale(){
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
					if(AddIfPotentialMove(downCase)){
						ret.Add(downCase);
					}
				}else{// if it's an ally
					influencingCases.Add(downCase);
				}
			}
			else if(AddIfPotentialMove(downCase)){
				ret.Add(downCase);
			}
		}
		if(indexCorner%8 != 0){ //if we are NOT on LEFT bounds
			leftCase = gameManager.GetCaseWithIndex(indexCorner -1);
			if(leftCase.isTaken()){ //if there's another piece on the case
				if(leftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(AddIfPotentialMove(leftCase)){
						ret.Add(leftCase);
					}
				}else{// if it's an ally
					influencingCases.Add(leftCase);
				}
			}
			else if(AddIfPotentialMove(leftCase)){
				ret.Add(leftCase);
			}
		}
		return ret;
	}

	private bool checkKnightUpLeftDiagonale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 7) // if we are on the LEFT bounds or TOP bounds
			return false;

		int indexCorner = actualIndex + 7;
		GameManager gameManager = GameManager.instance;
		Case upCase, leftCase;
		if(indexCorner/8 != 7){ //if we are NOT on TOP bounds
			upCase = gameManager.GetCaseWithIndex(indexCorner + 8);
			if(upCase.isTaken()){ //if there's another piece on the case
				if(upCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = upCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		if(indexCorner%8 != 0){ //if we are NOT on LEFT bounds
			leftCase = gameManager.GetCaseWithIndex(indexCorner -1);
			if(leftCase.isTaken()){ //if there's another piece on the case
				if(leftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = leftCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		return false;
	}

	private bool checkKnightUpRightDiagonale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 7) // if we are on the RIGHT bounds or TOP bounds
			return false;

		int indexCorner = actualIndex + 9;
		GameManager gameManager = GameManager.instance;
		Case upCase, rightCase;
		if(indexCorner/8 != 7){ //if we are NOT on TOP bounds
			upCase = gameManager.GetCaseWithIndex(indexCorner + 8);
			if(upCase.isTaken()){ //if there's another piece on the case
				if(upCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = upCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		if(indexCorner%8 != 7){ //if we are NOT on RIGHT bounds
			rightCase = gameManager.GetCaseWithIndex(indexCorner +1);
			if(rightCase.isTaken()){ //if there's another piece on the case
				if(rightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = rightCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		return false;
	}

	private bool checkKnightDownRightDiagonale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 0) // if we are on the RIGHT bounds or BOTTOM bounds
			return false;

		int indexCorner = actualIndex - 7;
		GameManager gameManager = GameManager.instance;
		Case downCase, rightCase;
		if(indexCorner/8 != 0){ //if we are NOT on BOTTOM bounds
			downCase = gameManager.GetCaseWithIndex(indexCorner - 8);
			if(downCase.isTaken()){ //if there's another piece on the case
				if(downCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = downCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		if(indexCorner%8 != 7){ //if we are NOT on RIGHT bounds
			rightCase = gameManager.GetCaseWithIndex(indexCorner +1);
			if(rightCase.isTaken()){ //if there's another piece on the case
				if(rightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = rightCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		return false;
	}

	private bool checkKnightDownLeftDiagonale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 0) // if we are on the LEFT bounds or BOTTOM bounds
			return false;

		int indexCorner = actualIndex - 9;
		GameManager gameManager = GameManager.instance;
		Case downCase, leftCase;
		if(indexCorner/8 != 0){ //if we are NOT on BOTTOM bounds
			downCase = gameManager.GetCaseWithIndex(indexCorner - 8);
			if(downCase.isTaken()){ //if there's another piece on the case
				if(downCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = downCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		if(indexCorner%8 != 7){ //if we are NOT on RIGHT bounds
			leftCase = gameManager.GetCaseWithIndex(indexCorner -1);
			if(leftCase.isTaken()){ //if there's another piece on the case
				if(leftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = leftCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		return false;
	}
/*
	protected override IEnumerator ShortRangeAttack(Case targetCase){
		yield return null;
	}

	protected override IEnumerator LongRangeAttack(Case targetCase){
		yield return null;
	}
	*/

}
