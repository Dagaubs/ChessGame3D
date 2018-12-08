using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece {

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			return GameManager.instance.GetCaseWithIndex(4);
		}else{ //black
			return GameManager.instance.GetCaseWithIndex(60);
		}
	}
}
