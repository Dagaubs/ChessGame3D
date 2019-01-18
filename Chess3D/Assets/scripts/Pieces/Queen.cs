using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece {

	public override string toString(){return player.getSide().ToString() + " QUEEN";}

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			return GameManager.instance.GetCaseWithIndex(3);
		}else{ //black
			return GameManager.instance.GetCaseWithIndex(59);
		}
	}

	public override bool CheckForCheck(){
		bool ret = false;
		ret = ret || checkUpLeftDiagonale();
		ret = ret || checkUpRightDiagonale();
		ret = ret || checkDownRightDiagonale();
		ret = ret || checkDownLeftDiagonale();
		ret = ret || checkUpVerticale();
		ret = ret || checkDownVerticale();
		ret = ret || checkLeftHorizontale();
		ret = ret || checkRightHorizontale();
		return ret;
	}

	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
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

	void Awake(){
		type = PieceType.QUEEN;
	}
}
