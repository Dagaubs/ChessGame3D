using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece {

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			return GameManager.instance.GetCaseWithIndex(3);
		}else{ //black
			return GameManager.instance.GetCaseWithIndex(59);
		}
	}
}
