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

	public override bool CheckForCheck(){
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
							string piecename = foundCase.GetStandingOnPiece().toString();
							if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
								return true;
						}
					}
				}
			}
		}
		return false;
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
							if(AddIfPotentialMove(foundCase))
								ret.Add(foundCase);
						} else{
							influencingCases.Add(foundCase);
						}
					}
					else if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
				}
			}
		}
		if(!_hasMoved){
			List<Case> castlingCases = LookForCastling(actualIndex);
			for(int i = 0; i< castlingCases.Count; ++i){
				ret.Add(castlingCases[i]);
			}
		}
		accessibleCases = ret;
	}

	protected List<Case> LookForCastling(int actualIndex){
		List<Case> castlingCases = new List<Case>();
		List<Piece> othersPiece = player.alivedPieces;
		foreach(Piece p in othersPiece){
			if(p.getType() == PieceType.ROOK && !p.HasMoved()){
				int rookIndex = p.getActualCase().GetIndex();
				if(rookIndex > actualIndex){
					bool isOk =true;
					for(int i = actualIndex+1; i<rookIndex; ++i){
						Case foundCase = GameManager.instance.GetCaseWithIndex(i);
						if(foundCase.isTaken() || !AddIfPotentialMove(foundCase)){
							isOk = false;
							break;
						}
						influencingCases.Add(foundCase);
					}
					if(isOk){
						Case castlingCase = GameManager.instance.GetCaseWithIndex(actualIndex+2);
						castlingCases.Add(castlingCase);
					}
				}
				else{
					bool isOk =true;
					for(int i = actualIndex-1; i>rookIndex; --i){
						Case foundCase = GameManager.instance.GetCaseWithIndex(i);
						if(foundCase.isTaken() || !AddIfPotentialMove(foundCase)){
							isOk = false;
							break;
						}
					}
					if(isOk){
						Case castlingCase = GameManager.instance.GetCaseWithIndex(actualIndex-2);
						if(AddIfPotentialMove(castlingCase))
							castlingCases.Add(castlingCase);
						else 
							Debug.Log("Can't castling cause it would be in check state !");
					}					
				}
			}
		} 
		return castlingCases;
	}

	void Awake(){
		type = PieceType.KING;
	}
}
