using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece {

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(2);
			else
				return GameManager.instance.GetCaseWithIndex(5);
		}else{ //black
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(58);
			else
				return GameManager.instance.GetCaseWithIndex(61);
		}
	}

	public override bool CheckForCheck(){
		bool ret = false;
		ret = ret || checkUpLeftDiagonale();
		ret = ret || checkUpRightDiagonale();
		ret = ret || checkDownRightDiagonale();
		return ret || checkDownLeftDiagonale();
	}
	
	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
		List<Case> ret = getUpLeftDiagonale();
		ret.AddRange(getUpRightDiagonale());
		ret.AddRange(getDownRightDiagonale());
		ret.AddRange(getDownLeftDiagonale());
		accessibleCases = ret;
	}

	public override string toString(){return player.getSide().ToString() + " BISHOP (" + piece_type_index + ")";}

}
