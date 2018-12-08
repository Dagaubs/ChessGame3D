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

	protected override void LookForAccessibleCases(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();
		int index;
		GameManager gameManager = GameManager.instance;
		Case foundCase;
		for(int y = -1; y <= 1; y++){
			for(int x = -1; x <= 1; x++){
				if(!(x == -1 && actualIndex%8 == 0) && !(y == -1 && actualIndex/8 ==0) && !(x == 1 && actualIndex%8==7) && !(y == 1 && actualIndex/8 == 7)){
					index = actualIndex + (x*1) + (y*8);
					foundCase = gameManager.GetCaseWithIndex(index);
					if(foundCase.isTaken()){ //if there's another piece on the case
						if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
							accessibleCases = ret;
							return;
						}else{
							ret.Add(foundCase);
							accessibleCases = ret;
							return;
						}
					}
					ret.Add(foundCase);
				}
			}
		}
		accessibleCases = ret;
	}
}
