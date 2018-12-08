using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
				return GameManager.instance.GetCaseWithIndex(7 + piece_type_index);
		}else{ //black
				return GameManager.instance.GetCaseWithIndex(47 + piece_type_index);
		}
	}

	protected override void LookForAccessibleCases(){
		if(player.getSide() == Player.PlayerSide.WHITE)
			white_LookForAccessibleCases();
		else
			black_LookForAccessibleCases();
		
	}

	private void white_LookForAccessibleCases(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 7){//If we are on TOP bounds 
			// SHOULD BE ABLE TO TRANSFORM IN QUEEN, KNIGHT OR ROOK
			accessibleCases = ret;
		}

		GameManager gameManager = GameManager.instance;
		Case upLeftCase, upRightCase, forwardCase, longForwardCase;
		if(actualCase == getInitialCase()){
			longForwardCase = gameManager.GetCaseWithIndex(actualIndex+16); // is able to move 2 case forward
			if(!longForwardCase.isTaken()) //if there's NO other piece on the case
				ret.Add(longForwardCase);
		}

		forwardCase = gameManager.GetCaseWithIndex(actualIndex+8);
		if(!forwardCase.isTaken()) //if there's NO other piece on the case
			ret.Add(forwardCase);

		if(actualIndex%8 != 0){ // if we are NOT on LEFT bounds
			upLeftCase = gameManager.GetCaseWithIndex(actualIndex+7);
			if(upLeftCase.isTaken()){ //if there's NO other piece on the case
				if(upLeftCase.GetStandingOnPiece().GetPlayer() != player)// if it's an enemy
					ret.Add(upLeftCase);
			}
		}

		if(actualIndex%8 != 7){ // if we are NOT on RIGHT bounds
			upRightCase = gameManager.GetCaseWithIndex(actualIndex+9);
			if(upRightCase.isTaken()){ //if there's NO other piece on the case
				if(upRightCase.GetStandingOnPiece().GetPlayer() != player)// if it's an enemy
					ret.Add(upRightCase);
			}
		}
		accessibleCases = ret;
	}

	private void black_LookForAccessibleCases(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 0){//If we are on BOTTOM bounds 
			// SHOULD BE ABLE TO TRANSFORM IN QUEEN, KNIGHT OR ROOK
			accessibleCases = ret;
		}

		GameManager gameManager = GameManager.instance;
		Case downLeftCase, downRightCase, forwardCase, longForwardCase;
		if(actualCase == getInitialCase()){
			longForwardCase = gameManager.GetCaseWithIndex(actualIndex-16); // is able to move 2 case forward
			if(!longForwardCase.isTaken()) //if there's NO other piece on the case
				ret.Add(longForwardCase);
		}

		forwardCase = gameManager.GetCaseWithIndex(actualIndex-8);
		if(!forwardCase.isTaken()) //if there's NO other piece on the case
			ret.Add(forwardCase);

		if(actualIndex%8 != 0){ // if we are NOT on LEFT bounds
			downLeftCase = gameManager.GetCaseWithIndex(actualIndex-9);
			if(downLeftCase.isTaken()){ //if there's NO other piece on the case
				if(downLeftCase.GetStandingOnPiece().GetPlayer() != player)// if it's an enemy
					ret.Add(downLeftCase);
			}
		}

		if(actualIndex%8 != 7){ // if we are NOT on RIGHT bounds
			downRightCase = gameManager.GetCaseWithIndex(actualIndex-7);
			if(downRightCase.isTaken()){ //if there's NO other piece on the case
				if(downRightCase.GetStandingOnPiece().GetPlayer() != player)// if it's an enemy
					ret.Add(downRightCase);
			}
		}
		accessibleCases = ret;
	}
}
