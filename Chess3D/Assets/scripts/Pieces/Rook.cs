using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece {

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
}
