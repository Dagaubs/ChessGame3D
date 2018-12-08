using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public enum PlayerSide{
		WHITE,
		BLACK
	}

	private bool playersTurn = false;
	public bool isPlaying(){ return playersTurn;}

	private PlayerSide side;
	public PlayerSide getSide(){return side;}
	private string toString(){return side.ToString()+ "PLAYER";}
	//private Camera mainCamera;
	[SerializeField]
	private LayerMask layerMask;
	public List<Piece> alivedPieces;
	public List<Piece> lostPieces;

	private bool picked = false;
	private Piece pickedPiece = null;

	public void LoseThisPiece(Piece p){
		if(alivedPieces.Contains(p) && !lostPieces.Contains(p)){
			alivedPieces.Remove(p);
			lostPieces.Add(p);
			EventManager.TriggerEvent(toString()+"_LOST_PIECE");
		}else{
			Debug.LogError("Can't lose this piece : not in alivelist! | is this in lostList ? => " + lostPieces.Contains(p));
		}
	}

	public void Init(PlayerSide s,Transform pieces_holder){
		side = s;
		createPieces(pieces_holder);
		//mainCamera = Camera.main; 
		layerMask = LayerMask.GetMask("Piece", "Case");
		startListenings();
	}

	private void startListenings(){
		//Debug.Log(side.ToString() + " starts listening");
		EventManager.StartListening(side.ToString()+"_BEGIN_TURN", beginOfTurn);
	}

	private void stopListenings(){
		EventManager.StopListening(side.ToString()+"_BEGIN_TURN", beginOfTurn);
	}

	private void beginOfTurn(){
		Debug.Log(toString()+" : begin turn");
		playersTurn = true;
	}

	private void endOfTurn(){
		playersTurn = false;
		EventManager.TriggerEvent(side.ToString()+"_END_TURN");
	}

	void Update(){
		if(playersTurn){
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
							pickedPiece.GoTo(hitCase);
							picked = false;
							pickedPiece = null;
							endOfTurn();
						}
						else if(hitCase.isTaken()){
							foundPiece = hitCase.GetStandingOnPiece();
							if(foundPiece.GetPlayer() == this){
								//Debug.Log(toString() + " : pick an ally !");
								if(picked)
									pickedPiece.Unpick();
								
								picked = true;
								pickedPiece = foundPiece;
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
								foundPiece.Pick();
							}
					}else{
						Debug.LogError("You shouldn't be able to hit that : " + hit.collider.gameObject + " | layer : " + LayerMask.LayerToName(hit.collider.gameObject.layer));
					}
				}
			}
		}
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
