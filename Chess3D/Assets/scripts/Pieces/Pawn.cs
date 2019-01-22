using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {

	public override string toString(){return player.getSide().ToString() + " PAWN (" + piece_type_index + ")";}

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
				return GameManager.instance.GetCaseWithIndex(7 + piece_type_index);
		}else{ //black
				return GameManager.instance.GetCaseWithIndex(47 + piece_type_index);
		}
	}

	public override bool CheckForCheck(){
		if(player.getSide() == Player.PlayerSide.WHITE)
			return white_CheckForCheck();
		else
			return black_CheckForCheck();
		
	}

	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
		if(player.getSide() == Player.PlayerSide.WHITE)
			white_LookForAccessibleCases();
		else
			black_LookForAccessibleCases();
		
	}

	private void white_LookForAccessibleCases(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();
		GameManager gameManager = GameManager.instance;

		if(actualIndex/8 == 7){//If we are on TOP bounds 
			// SHOULD BE ABLE TO TRANSFORM IN QUEEN, KNIGHT OR ROOK
			accessibleCases = ret;
			gameManager.PawnToQueen(this, actualCase);
			return;
		}

		Case upLeftCase, upRightCase, forwardCase, longForwardCase;
		Case initialCase = getInitialCase();
		if(actualCase == initialCase){
			longForwardCase = gameManager.GetCaseWithIndex(actualIndex+16); // is able to move 2 case forward
			if(!longForwardCase.isTaken()) //if there's NO other piece on the case
				if(AddIfPotentialMove(longForwardCase))
					ret.Add(longForwardCase);
			else
				influencingCases.Add(longForwardCase);
		}
	 	else if(actualIndex == initialCase.GetIndex() +24){ //prise en passant
			Case rightCase = gameManager.GetCaseWithIndex(actualIndex+1);
			Case leftCase = gameManager.GetCaseWithIndex(actualIndex-1);
			influencingCases.Add(rightCase);
			influencingCases.Add(leftCase);
			Move lastMove = gameManager.GetLastMove();
			Piece lastPieceMoved = lastMove.getMovedPiece();
			if(lastPieceMoved.getType() == PieceType.PAWN){
				Pawn pawnMoved = (Pawn) lastPieceMoved;
				if(lastMove.getLeftCase() == pawnMoved.getInitialCase()){
					Case caseJoinedatLastMove = lastMove.getJoinedCase();
					if(caseJoinedatLastMove.GetIndex() == actualIndex +1){
						ret.Add(gameManager.GetCaseWithIndex(actualIndex+9));
					}
					else if(caseJoinedatLastMove.GetIndex() == actualIndex -1){
						ret.Add(gameManager.GetCaseWithIndex(actualIndex+7));
					}
				}
			}
		}

		forwardCase = gameManager.GetCaseWithIndex(actualIndex+8);
		if(!forwardCase.isTaken()) //if there's NO other piece on the case
			if(AddIfPotentialMove(forwardCase))
				ret.Add(forwardCase);
		else
			influencingCases.Add(forwardCase);

		if(actualIndex%8 != 0){ // if we are NOT on LEFT bounds
			upLeftCase = gameManager.GetCaseWithIndex(actualIndex+7);
			if(upLeftCase.isTaken()){ //if there's NO other piece on the case
				if(upLeftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(AddIfPotentialMove(upLeftCase))
						ret.Add(upLeftCase);
				}else{ //if it's a ally
					influencingCases.Add(upLeftCase);
				}
			}
			else{
					influencingCases.Add(upLeftCase);
			}
		}

		if(actualIndex%8 != 7){ // if we are NOT on RIGHT bounds
			upRightCase = gameManager.GetCaseWithIndex(actualIndex+9);
			if(upRightCase.isTaken()){ //if there's NO other piece on the case
				if(upRightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(AddIfPotentialMove(upRightCase))
						ret.Add(upRightCase);
				}else{ //if it's a ally
					influencingCases.Add(upRightCase);
				}
			}
			else{
					influencingCases.Add(upRightCase);
			}
		}
		accessibleCases = ret;
	}

	private void black_LookForAccessibleCases(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();
		Case initialCase = getInitialCase();
		GameManager gameManager = GameManager.instance;

		if(actualIndex/8 == 0){//If we are on TOP bounds 
			// SHOULD BE ABLE TO TRANSFORM IN QUEEN, KNIGHT OR ROOK
			accessibleCases = ret;
			gameManager.PawnToQueen(this, actualCase);
			return;
		}

		Case downLeftCase, downRightCase, forwardCase, longForwardCase;
		if(actualCase == initialCase){
			longForwardCase = gameManager.GetCaseWithIndex(actualIndex-16); // is able to move 2 case forward
			if(!longForwardCase.isTaken()) //if there's NO other piece on the case
				if(AddIfPotentialMove(longForwardCase))
					ret.Add(longForwardCase);
			else
				influencingCases.Add(longForwardCase);
		}
		else if(actualIndex == initialCase.GetIndex() -24){ //prise en passant
			Case rightCase = gameManager.GetCaseWithIndex(actualIndex+1);
			Case leftCase = gameManager.GetCaseWithIndex(actualIndex-1);
			influencingCases.Add(rightCase);
			influencingCases.Add(leftCase);
			Move lastMove = gameManager.GetLastMove();
			Piece lastPieceMoved = lastMove.getMovedPiece();
			if(lastPieceMoved.getType() == PieceType.PAWN){
				Pawn pawnMoved = (Pawn) lastPieceMoved;
				if(lastMove.getLeftCase() == pawnMoved.getInitialCase()){
					Case caseJoinedatLastMove = lastMove.getJoinedCase();
					if(caseJoinedatLastMove.GetIndex() == actualIndex +1){
						ret.Add(gameManager.GetCaseWithIndex(actualIndex-7));
					}
					else if(caseJoinedatLastMove.GetIndex() == actualIndex -1){
						ret.Add(gameManager.GetCaseWithIndex(actualIndex-9));
					}
				}
			}
		}

		forwardCase = gameManager.GetCaseWithIndex(actualIndex-8);
		if(!forwardCase.isTaken()) //if there's NO other piece on the case
			if(AddIfPotentialMove(forwardCase))
				ret.Add(forwardCase);
		else
			influencingCases.Add(forwardCase);

		if(actualIndex%8 != 0){ // if we are NOT on LEFT bounds
			downLeftCase = gameManager.GetCaseWithIndex(actualIndex-9);
			if(downLeftCase.isTaken()){ //if there's NO other piece on the case
				if(downLeftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(AddIfPotentialMove(downLeftCase))
						ret.Add(downLeftCase);
				}else{ //if it's a ally
					influencingCases.Add(downLeftCase);
				}
			}
			else{
					influencingCases.Add(downLeftCase);
			}
		}

		if(actualIndex%8 != 7){ // if we are NOT on RIGHT bounds
			downRightCase = gameManager.GetCaseWithIndex(actualIndex-7);
			if(downRightCase.isTaken()){ //if there's NO other piece on the case
				if(downRightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(AddIfPotentialMove(downRightCase))
						ret.Add(downRightCase);
				}else{ //if it's a ally
					influencingCases.Add(downRightCase);
				}
			}
			else{
					influencingCases.Add(downRightCase);
			}
		}
		accessibleCases = ret;
	}

	private bool white_CheckForCheck(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 7){//If we are on TOP bounds 
			return false;
		}

		GameManager gameManager = GameManager.instance;
		Case upLeftCase, upRightCase;/*, forwardCase, longForwardCase;
		
		if(actualCase == getInitialCase()){
			longForwardCase = gameManager.GetCaseWithIndex(actualIndex+16); // is able to move 2 case forward
			if(!longForwardCase.isTaken()) //if there's NO other piece on the case
				ret.Add(longForwardCase);
			else
				influencingCases.Add(longForwardCase);
		}

		forwardCase = gameManager.GetCaseWithIndex(actualIndex+8);
		if(!forwardCase.isTaken()) //if there's NO other piece on the case
			ret.Add(forwardCase);
		else
			influencingCases.Add(forwardCase);
		*/
		if(actualIndex%8 != 0){ // if we are NOT on LEFT bounds
			upLeftCase = gameManager.GetCaseWithIndex(actualIndex+7);
			if(upLeftCase.isTaken()){ //if there's NO other piece on the case
				if(upLeftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = upLeftCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}

		if(actualIndex%8 != 7){ // if we are NOT on RIGHT bounds
			upRightCase = gameManager.GetCaseWithIndex(actualIndex+9);
			if(upRightCase.isTaken()){ //if there's NO other piece on the case
				if(upRightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = upRightCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}
		return false;
	}

	private bool black_CheckForCheck(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 0){//If we are on BOTTOM bounds 
			return false;
		}

		GameManager gameManager = GameManager.instance;
		Case downLeftCase, downRightCase;/*, forwardCase, longForwardCase;
		if(actualCase == getInitialCase()){
			longForwardCase = gameManager.GetCaseWithIndex(actualIndex-16); // is able to move 2 case forward
			if(!longForwardCase.isTaken()) //if there's NO other piece on the case
				ret.Add(longForwardCase);
			else
				influencingCases.Add(longForwardCase);
		}

		forwardCase = gameManager.GetCaseWithIndex(actualIndex-8);
		if(!forwardCase.isTaken()) //if there's NO other piece on the case
			ret.Add(forwardCase);
		else
			influencingCases.Add(forwardCase);
		
		*/
		if(actualIndex%8 != 0){ // if we are NOT on LEFT bounds
			downLeftCase = gameManager.GetCaseWithIndex(actualIndex-9);
			if(downLeftCase.isTaken()){ //if there's NO other piece on the case
				if(downLeftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = downLeftCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
				}
			}
		}

		if(actualIndex%8 != 7){ // if we are NOT on RIGHT bounds
			downRightCase = gameManager.GetCaseWithIndex(actualIndex-7);
			if(downRightCase.isTaken()){ //if there's NO other piece on the case
				if(downRightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					string piecename = downRightCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
							return true;
				}
			}
		}
		return false;
	}

	void Awake(){
		type = PieceType.PAWN;
	}
}
