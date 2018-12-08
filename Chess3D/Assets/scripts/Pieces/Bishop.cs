﻿using System.Collections;
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
	
}
