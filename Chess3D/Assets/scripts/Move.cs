using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move {

	protected Case leftCase, joinedCase;
	protected Piece movedPiece, killedPiece;

	public Case getLeftCase(){return leftCase;}
	public Case getJoinedCase(){return joinedCase;}
	public Piece getMovedPiece(){return movedPiece;}
	public Piece getKilledPiece(){return killedPiece;}

	public Move(Case leftCase, Case joinedCase, Piece movedPiece, Piece killedPiece = null){
		this.leftCase = leftCase;
		this.joinedCase = joinedCase;
		this.movedPiece = movedPiece;
		this.killedPiece = killedPiece; 
	}

	public virtual string toString(){
		return movedPiece.toString() + " moved from " + leftCase.toString() + " and " + (killedPiece != null ? " destroyed " + killedPiece.toString() + " on " : " moved to ") + joinedCase.toString();
	}
}
