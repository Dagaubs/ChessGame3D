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
}
