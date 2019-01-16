using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

	[SerializeField]
	private Transform plateTransform, pieces_holder, whiteLosses, blackLosses;

	[SerializeField]
	private GameObject light_case, dark_case;

	public GameObject king, queen, rook, bishop, knight, pawn;

	public Material whiteMaterial, blackMaterial;

	private List<Move> moves;

	[SerializeField]
	private float case_size = 1f;

	private Case[] cases = null;

	public GameObject[] UIimagesPieceLost;
	public Text[]		UITextsPieceLost;
	public int[]		PiecesLostBySide;

	public Case GetCaseWithIndex(int index){
		if(index < 0 || index >= 64){
			Debug.LogError("GM_GetCaseWithIndex : index not correct : " + index);
			return null;
		}
		return cases[index];}

	private static GameManager gameManager;

	private Player whitePlayer, blackPlayer;
	public bool isInit = false, isInitiating = false, gameRunning = false;

	private Player.PlayerSide actualTurn = Player.PlayerSide.WHITE;

	public static GameManager instance
    {
        get
        {
            if (!gameManager)
            {
                gameManager = FindObjectOfType (typeof (GameManager)) as GameManager;

                if (!gameManager)
                {
                    Debug.LogError ("There needs to be one active GameManger script on a GameObject in your scene.");
                }
                else
                {
                    gameManager.Init(); 
                }
            }

            return gameManager;
        }
    }

	void Init ()
    {
		if(isInit || isInitiating) return;
		isInitiating = true;
		moves = new List<Move>();
        if (cases == null)
        {
			cases = new Case[64];
			initPlate();
        }
		//create player and make them spawn pieces
		whitePlayer = new GameObject("White player").AddComponent<Player>();
		whitePlayer.Init(Player.PlayerSide.WHITE, pieces_holder);
		blackPlayer = new GameObject("Black player").AddComponent<Player>();
		blackPlayer.Init(Player.PlayerSide.BLACK, pieces_holder);

		//remove accesibility to all Cases
		foreach(Case c in cases)
			c.setAccessibility(false);

		whitePlayer.RefreshAllPieces();
		blackPlayer.RefreshAllPieces();

		//initialise tabs
		PiecesLostBySide = new int[10]; // 10 => we don't count the king

		isInitiating = false;
		isInit = true;
    }

	public bool SaveNewPotentialMove(PotentialMove m){
		bool ret = false;

		foreach(Piece p in whitePlayer.alivedPieces){
			if(p.HasThisCaseInAccessiblesOrInfluence(m.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(m.getJoinedCase())){
				Debug.Log(p.toString() + " is checking for check!");
				ret = ret || p.CheckForCheck();
			}
		}
		foreach(Piece p in blackPlayer.alivedPieces){
			if(p.HasThisCaseInAccessiblesOrInfluence(m.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(m.getJoinedCase())){
				Debug.Log(p.toString() + " is checking for check!");
				ret = ret || p.CheckForCheck();
			}
		}

		m.ReverseMove();
		return ret;
	}

	public void SaveNewMove(Move m){
		moves.Add(m);
		foreach(Piece p in whitePlayer.alivedPieces){
			if(p.HasThisCaseInAccessiblesOrInfluence(m.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(m.getJoinedCase()))
				p.RefreshAccessible();
		}
		foreach(Piece p in blackPlayer.alivedPieces){
			if(p.HasThisCaseInAccessiblesOrInfluence(m.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(m.getJoinedCase()))
				p.RefreshAccessible();
		}
		Piece killedPiece = m.getKilledPiece();
		if(killedPiece != null){ //If a Piece was destroyed by this move
			killedPiece.GetEaten();

			//ANIMATION DE DEATH ?!

			Destroy(killedPiece.gameObject);
		//	Transform chosenTransform = killedPiece.GetPlayer().getSide() == Player.PlayerSide.WHITE ? whiteLosses : blackLosses;
		//	placeKilledPieceInGraveyard(killedPiece, chosenTransform);
		}
		m.getMovedPiece().RefreshAccessible();
		Debug.Log(m.toString());
		endOfActualTurn();
	}

/*	private void placeKilledPieceInGraveyard(Piece killedPiece, Transform chosenTransform){
		killedPiece.transform.SetParent(chosenTransform);
		Player ownPlayer = killedPiece.GetPlayer();
		killedPiece.transform.localPosition = Vector3.back * (ownPlayer.lostPieces.Count - 1);
	}*/

	void Start(){
		Init();
		startListenings();
		beginGame();
	}

	private void startListenings(){
		/*EventManager.StartListening("WHITE_END_TURN", endOfWhiteTurn);
		EventManager.StartListening("BLACK_END_TURN", endOfBlackTurn);
		EventManager.StartListening("WHITEPLAYER_LOST_PIECE", whitePlayerLostPiece);
		EventManager.StartListening("BLACKPLAYER_LOST_PIECE", blackPlayerLostPiece);*/
	}

	private void stopListenings(){/*
		EventManager.StopListening("WHITE_END_TURN", endOfWhiteTurn);
		EventManager.StopListening("BLACK_END_TURN", endOfBlackTurn);
		EventManager.StopListening("WHITEPLAYER_LOST_PIECE", whitePlayerLostPiece);
		EventManager.StopListening("BLACKPLAYER_LOST_PIECE", blackPlayerLostPiece);*/
	}

	private void endOfActualTurn(){
		Player.PlayerSide finishedTurn = actualTurn;
		if(actualTurn == Player.PlayerSide.WHITE){
			actualTurn = Player.PlayerSide.BLACK;
		}else{
			actualTurn = Player.PlayerSide.WHITE;
		}
		EventManager.TriggerEvent(finishedTurn.ToString() + "_END_TURN");
		beginNewturn();
	}

	private void endOfWhiteTurn(){
		if(actualTurn == Player.PlayerSide.WHITE){
			actualTurn = Player.PlayerSide.BLACK;
			beginNewturn();
		}else{
			Debug.LogError("Received WHITE_END_TURN event but it's BLACK turn!");
		}
	}

	private void endOfBlackTurn(){
		if(actualTurn == Player.PlayerSide.BLACK){
			actualTurn = Player.PlayerSide.WHITE;
			beginNewturn();
		}else{
			Debug.LogError("Received BLACK_END_TURN event but it's WHITE turn!");
		}
	}
/*
	private void whitePlayerLostPiece(){
		Piece freshlyLostPiece = whitePlayer.lostPieces[whitePlayer.lostPieces.Count-1];
		freshlyLostPiece.transform.SetParent(whiteLosses, false);
	}*/

	public void PlayerLostPiece(Player.PlayerSide side, Piece.PieceType type){
		int offset = side == Player.PlayerSide.WHITE ? 0 : 5;
		int index = offset + (int)type -1; //-1 => we don't count the king

		PiecesLostBySide[index]++;
		UIimagesPieceLost[index].SetActive(true);

		if(PiecesLostBySide[index] > 1){
			UITextsPieceLost[index].text = "x"+PiecesLostBySide[index];
		}
	}
/* 
	private void blackPlayerLostPiece(){
		Piece freshlyLostPiece = blackPlayer.lostPieces[blackPlayer.lostPieces.Count-1];
		freshlyLostPiece.transform.SetParent(blackLosses, false);
	}*/

	private void beginGame(){
		// TODO: set up UI

		gameRunning = true;
		beginNewturn();
	}

	private void beginNewturn(){
		// TODO: modify UI

		//Debug.Log("new turn : " + actualTurn.ToString());
		EventManager.TriggerEvent(actualTurn.ToString() + "_BEGIN_TURN");
	}

	private void initPlate(){
		bool isLight = true;
		for(int i = 0; i < 64; i++){
			GameObject caseGo = isLight ? light_case : dark_case;
			Case createCase = Instantiate(caseGo, plateTransform, false).GetComponent<Case>();
			createCase.init(i, case_size);
			cases[i] = createCase;
			if((i+1) % 8 != 0)
				isLight = !isLight;
		}
	}

	public void SelectThisCases(List<Case> casesToSelect){

	}

	void Update(){/*
		if(gameRunning){

		}*/
	}
}
