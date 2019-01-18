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

	public override bool CheckForCheck(){
		bool ret = false;
		ret = ret || checkUpVerticale();
		ret = ret || checkDownVerticale();
		ret = ret || checkLeftHorizontale();
		return ret || checkRightHorizontale();
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

	void Awake(){
		type = PieceType.ROOK;
	}
}
