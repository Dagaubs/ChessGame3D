using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece {

	public override string toString(){return player.getSide().ToString() + " KING";}

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			return GameManager.instance.GetCaseWithIndex(4);
		}else{ //black
			return GameManager.instance.GetCaseWithIndex(60);
		}
	}

	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
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
						if(foundCase.GetStandingOnPiece().GetPlayer() != player){// if it's an enemy
							Debug.Log(player.toString() + " : Found a case with an enemy : " + foundCase.GetStandingOnPiece().GetPlayer().toString());
							ret.Add(foundCase);
						}else{
							influencingCases.Add(foundCase);
						}
					}
					else{
						ret.Add(foundCase);
					}
				}
			}
		}
		accessibleCases = ret;
	}
}
