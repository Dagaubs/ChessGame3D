using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece {

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(1);
			else
				return GameManager.instance.GetCaseWithIndex(6);
		}else{ //black
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(57);
			else
				return GameManager.instance.GetCaseWithIndex(62);
		}
	}
}
