using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotentialMove : Move {

	public PotentialMove(Case leftCase, Case joinedCase, Piece movedPiece, Piece killedPiece = null): base(leftCase, joinedCase, movedPiece, killedPiece){

	}

	public override void ReverseMove(bool FromReverse = false){
		movedPiece.ReversePotentiallyGoTo(leftCase, joinedCase, killedPiece);
	}

	public override string toString(){
		return movedPiece.toString() + (killedPiece != null ? " could destroyed " + killedPiece.toString() + " on " : " could moved to ") + joinedCase.toString();
	}
}
