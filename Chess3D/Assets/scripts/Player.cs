using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public enum PlayerSide{
		WHITE,
		BLACK
	}

	private PlayerSide side;
	public PlayerSide getSide(){return side;}

	public List<Piece> alivedPieces;

	public void Init(PlayerSide s,Transform pieces_holder){
		side = s;
		createPieces(pieces_holder);
	}

	private void createPieces(Transform pieces_holder){
		alivedPieces = new List<Piece>();

		Piece king = Instantiate(GameManager.instance.king, pieces_holder, false).GetComponent<Piece>();
		king.Init(this);
		alivedPieces.Add(king);

		Piece queen = Instantiate(GameManager.instance.queen, pieces_holder, false).GetComponent<Piece>();
		queen.Init(this);
		alivedPieces.Add(queen);

		Piece rook1 = Instantiate(GameManager.instance.rook, pieces_holder, false).GetComponent<Piece>();
		rook1.setIndex(1);
		rook1.Init(this);
		alivedPieces.Add(rook1);

		Piece rook2 = Instantiate(GameManager.instance.rook, pieces_holder, false).GetComponent<Piece>();
		rook2.setIndex(2);
		rook2.Init(this);
		alivedPieces.Add(rook2);

		Piece bishop1 = Instantiate(GameManager.instance.bishop, pieces_holder, false).GetComponent<Piece>();
		bishop1.setIndex(1);
		bishop1.Init(this);
		alivedPieces.Add(bishop1);

		Piece bishop2 = Instantiate(GameManager.instance.bishop, pieces_holder, false).GetComponent<Piece>();
		bishop2.setIndex(2);
		bishop2.Init(this);
		alivedPieces.Add(bishop2);

		Piece knight1 = Instantiate(GameManager.instance.knight, pieces_holder, false).GetComponent<Piece>();
		knight1.setIndex(1);
		knight1.Init(this);
		alivedPieces.Add(knight1);

		Piece knight2 = Instantiate(GameManager.instance.knight, pieces_holder, false).GetComponent<Piece>();
		knight2.setIndex(2);
		knight2.Init(this);
		alivedPieces.Add(knight2);

		for(int pawn_index = 1; pawn_index <= 8; pawn_index++){
			Piece pawn = Instantiate(GameManager.instance.pawn, pieces_holder, false).GetComponent<Piece>();
			pawn.setIndex(pawn_index);
			pawn.Init(this);
			alivedPieces.Add(pawn);
		}
	}
}
