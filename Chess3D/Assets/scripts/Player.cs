using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public enum PlayerSide{
		WHITE,
		BLACK
	}

	[SerializeField]
	private bool playersTurn = false;
	public bool isPlaying(){ return playersTurn;}

	[SerializeField]
	private Text timeText;

	private float timeInSec;

	private PlayerSide side;
	public PlayerSide getSide(){return side;}
	public string toString(){return side.ToString()+ "PLAYER";}
	
	[SerializeField]
	private LayerMask layerMask;
	public List<Piece> alivedPieces;

	private King kingPiece;
	public King GetKing(){return kingPiece;}

	private bool picked = false;
	private Piece pickedPiece = null;

	public void LoseThisPiece(Piece piece){
		if(alivedPieces.Contains(piece))
        {
            GameManager.AddPointsForKillOf(piece);
            alivedPieces.Remove(piece);
			GameManager.instance.PlayerLostPiece(side, piece.getType());
		}else{
			Debug.LogError("Can't lose this piece : not in alivelist! ");
		}
	}

	public void Init(PlayerSide s,Transform pieces_holder, float timeInSec, Text text){
		side = s;
		createPieces(pieces_holder);
		this.timeInSec = timeInSec;
		this.timeText = text;
		DisplayActualTime();
		layerMask = LayerMask.GetMask("Piece", "Case");
	}

	private void DisplayActualTime(){
		int nbMinutes = (int)timeInSec / 60;
		int nbSec = (int)timeInSec % 60;
		timeText.text = (nbMinutes < 10 ? "0"+nbMinutes.ToString() : nbMinutes.ToString()) + ":" + (nbSec < 10 ? "0"+nbSec.ToString() : nbSec.ToString()); 
	}

	public void RefreshAllPieces(){
		foreach(Piece p in alivedPieces){
			p.RefreshAccessible();
		}
	}

	public void yourTurn(bool canPlay)
	{
		playersTurn = canPlay;
		if(playersTurn){
			foreach(Piece p in alivedPieces){
				if(p.CanMove())
					return;
			}
			// if no piece can Move : Checkmate
			GameManager.instance.EndOfGame(side != PlayerSide.WHITE);
		}else{
			timeInSec += GameManager.TimeToAdd;
			// todo anim show more time was given
			DisplayActualTime();
		}
	}


	void Update(){
		if(playersTurn && GameManager.ActualState == GameManager.STATE.RUNNING){
			timeInSec-= Time.deltaTime;
			DisplayActualTime();
			if(timeInSec <= 0){
				GameManager.instance.EndOfGame(side == PlayerSide.BLACK);
			}
			if (Input.GetMouseButtonUp(0))
			{
				//Debug.Log(toString() + " : fire !");
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray,out hit, layerMask)){
					Piece foundPiece;
					//Debug.Log("Hit something : " + hit.collider.gameObject.name);
					if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Case")){// if you hit a Case
						Case hitCase = hit.collider.GetComponent<Case>();
						if(picked && hitCase.isAccessible()){
							//Create class deplacement and save it
							//Debug.Log(toString() + " : Found a case accessible => " + hitCase.GetIndex());
							pickedPiece.UnpickExceptTarget(hitCase);
							Move savedMove = pickedPiece.GoTo(hitCase);
							if(savedMove != null){

								picked = false;
								pickedPiece = null;
								GameManager.instance.SaveNewMove(savedMove);
 							}else{
								Debug.LogError("Move null : case " + hitCase.GetIndex() + " was not accessible!");
							}
						}
						else if(hitCase.isTaken()){
							foundPiece = hitCase.GetStandingOnPiece();
							if(foundPiece.GetPlayer() == this){
								//Debug.Log(toString() + " : pick an ally !");
								if(picked)
									pickedPiece.Unpick();
								
								picked = true;
								pickedPiece = foundPiece;
								foundPiece.RefreshAccessible();
								foundPiece.Pick();
							}
						}
					}else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece")){ // if you hit a Piece
							foundPiece = hit.collider.GetComponent<Piece>();
							if(foundPiece.GetPlayer() == this){
								if(picked)
									pickedPiece.Unpick();
									
								picked = true;
								pickedPiece = foundPiece;
								foundPiece.RefreshAccessible();
								foundPiece.Pick();
							}
					}else{
						//Debug.LogError("You shouldn't be able to hit that : " + hit.collider.gameObject + " | layer : " + LayerMask.LayerToName(hit.collider.gameObject.layer));
					}
				}
			}
		}
	}


    public void SpawnPieceOn(Piece.PieceType type, Case joinedCase)
    {
        GameObject prefab = GameManager.GetPrefab(type);
        Piece newPiece = Instantiate(prefab, GameManager.PieceHolder, false).GetComponent<Piece>();
        newPiece.Init(this, joinedCase);
        alivedPieces.Add(newPiece);
    }

    private void createPieces(Transform pieces_holder){
		alivedPieces = new List<Piece>();

		Piece king = Instantiate(GameManager.instance.king, pieces_holder, false).GetComponent<Piece>();
		king.Init(this);
		kingPiece = king as King;
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

	public void DetroyAllPieces(){
		foreach(Piece p in alivedPieces){
			Case c = p.getActualCase();
			c.LeavePiece();
			Destroy(p.gameObject);
		}
	}

    public void PieceResurected(Piece piece)
    {
        if (!alivedPieces.Contains(piece))
        {
            alivedPieces.Add(piece);
            GameManager.RemovePointsForKillOf(piece);
        }
        else
        {
            Debug.LogError(piece.toString() + " is already in alived Pieces ! ");
        }
    }
}
