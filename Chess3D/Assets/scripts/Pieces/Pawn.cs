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
			gameManager.PawnToQueen(this);
			return;
		}

		Case upLeftCase, upRightCase, forwardCase, longForwardCase;
		Case initialCase = getInitialCase();

		forwardCase = gameManager.GetCaseWithIndex(actualIndex+8);
		if(!forwardCase.isTaken()) //if there's NO other piece on the case
			if(AddIfPotentialMove(forwardCase))
				ret.Add(forwardCase);
		else
			influencingCases.Add(forwardCase);

		if(actualCase == initialCase && !forwardCase.isTaken()){
			longForwardCase = gameManager.GetCaseWithIndex(actualIndex+16); // is able to move 2 case forward
			if(!longForwardCase.isTaken()) //if there's NO other piece on the case
				if(AddIfPotentialMove(longForwardCase))
					ret.Add(longForwardCase);
			else
				influencingCases.Add(longForwardCase);
		}
	 	else if(actualIndex/8 == 4){ //prise en passant
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
						Case foundCase = gameManager.GetCaseWithIndex(actualIndex+9);
						if(AddIfPotentialMove(foundCase))
							ret.Add(foundCase);
					}
					else if(caseJoinedatLastMove.GetIndex() == actualIndex -1){
						Case foundCase = gameManager.GetCaseWithIndex(actualIndex+7);
						if(AddIfPotentialMove(foundCase))
							ret.Add(foundCase);
					}
				}
			}
		}

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
			gameManager.PawnToQueen(this);
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
		else if(actualIndex/8 == 3){ //prise en passant
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
						Case foundCase = gameManager.GetCaseWithIndex(actualIndex-7);
						if(AddIfPotentialMove(foundCase))
							ret.Add(foundCase);
					}
					else if(caseJoinedatLastMove.GetIndex() == actualIndex -1){
						Case foundCase = gameManager.GetCaseWithIndex(actualIndex-9);
						if(AddIfPotentialMove(foundCase))
							ret.Add(foundCase);
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
		Case upLeftCase, upRightCase;
		
		if(actualIndex%8 != 0){ // if we are NOT on LEFT bounds
			upLeftCase = gameManager.GetCaseWithIndex(actualIndex+7);
			if(upLeftCase.isTaken()){ //if there's NO other piece on the case
				if(upLeftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(upLeftCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
						return true;
				}
			}
		}

		if(actualIndex%8 != 7){ // if we are NOT on RIGHT bounds
			upRightCase = gameManager.GetCaseWithIndex(actualIndex+9);
			if(upRightCase.isTaken()){ //if there's NO other piece on the case
				if(upRightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(upRightCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
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
		Case downLeftCase, downRightCase;

		if(actualIndex%8 != 0){ // if we are NOT on LEFT bounds
			downLeftCase = gameManager.GetCaseWithIndex(actualIndex-9);
			if(downLeftCase.isTaken()){ //if there's NO other piece on the case
				if(downLeftCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(downLeftCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
						return true;
				}
			}
		}

		if(actualIndex%8 != 7){ // if we are NOT on RIGHT bounds
			downRightCase = gameManager.GetCaseWithIndex(actualIndex-7);
			if(downRightCase.isTaken()){ //if there's NO other piece on the case
				if(downRightCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
					if(downRightCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
							return true;
				}
			}
		}
		return false;
	}

	void Awake(){
		type = PieceType.PAWN;
	}

	/*protected override IEnumerator ShortRangeAttack(Case targetCase){
		transform.LookAt(targetCase.GetStandingOnPieceTransform());

		Vector3 targetMovePosition = targetCase.GetAttackPosition(this);
		Vector3 velocity = new Vector3();
		float actualSpeed = 0f;
		// Move To targetMovePosition
		while(Vector3.Distance(targetMovePosition, transform.position) > 0.1f){

			if(Vector3.Distance(targetMovePosition, transform.position) < 1f && actualSpeed > 2f)
				actualSpeed -= acceleration * Time.deltaTime;
			else
				actualSpeed += acceleration * Time.deltaTime;
			actualSpeed = actualSpeed > maxSpeed ? maxSpeed : actualSpeed;
			_animator.SetFloat("Speed", actualSpeed);
			velocity = Vector3.forward * actualSpeed;
			transform.Translate(velocity * Time.deltaTime);
			if(Vector3.Distance(Vector3.Normalize(targetMovePosition - transform.position), transform.forward) > 0.1f){
				//Debug.Log("Should be passed : " + Vector3.Normalize(targetMovePosition - transform.position) + " | forward : " + transform.forward);
				_animator.SetFloat("Speed", 0f);
				transform.position = targetMovePosition;
			}
			yield return new WaitForFixedUpdate();
		}
		_animator.SetFloat("Speed", 0f);
		Debug.Log("triggering attack !");
		transform.LookAt(enemyPieceAttacked.transform);
		_animator.SetTrigger("Attack");
	}

	protected override IEnumerator LongRangeAttack(Case targetCase){
		StartCoroutine(ShortRangeAttack(targetCase));
		yield return null;
	}*/

}
