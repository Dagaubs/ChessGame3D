﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    public enum STATE
    {
        INITIATING,
        RUNNING,
        LOOKINGBACKWARD
    }

    private STATE actualState = STATE.INITIATING;

    public static STATE ActualState
    {
        get { return instance.actualState; }
        private set { instance.actualState = value; }
    }

    public void SetActualState(STATE newState)
    {
        actualState = newState;
    }

	[SerializeField]
	private Transform plateTransform, pieces_holder;

    [SerializeField]
    private PanelMoves PanelMoves;

	[SerializeField]
	private GameObject light_case, dark_case, transform_circle;

	[SerializeField]
	private Text whiteTimeText, blackTimeText, whitePointText, blackPointText;

    private int actualPoint;

    public static int Point // if > 0 White winning, otherwise black winning
    {
        get { return instance.actualPoint; }
        private set {
            instance.actualPoint = value;
            instance.whitePointText.text = GetTextForPointValue(value);
            instance.blackPointText.text = GetTextForPointValue(-value);
        }
    }
	public GameObject king, queen, rook, bishop, knight, pawn;

    public static Dictionary<Piece.PieceType, int> PieceValues = new Dictionary<Piece.PieceType, int>() {
        { Piece.PieceType.PAWN, 1}, { Piece.PieceType.QUEEN, 9}, { Piece.PieceType.KNIGHT, 3}, { Piece.PieceType.BISHOP, 3}, { Piece.PieceType.ROOK, 5}
    };

    private static string GetTextForPointValue(int value)
    {
        return (Mathf.Sign(value) >= 1 ? "+ " : "- ") + Mathf.Abs(value).ToString();
    }

    public static GameObject GetPrefab(Piece.PieceType type)
    {
        switch (type)
        {
            case Piece.PieceType.PAWN:
                return instance.pawn;
            case Piece.PieceType.QUEEN:
                return instance.queen;
            case Piece.PieceType.BISHOP:
                return instance.bishop;
            case Piece.PieceType.KNIGHT:
                return instance.knight;
            case Piece.PieceType.ROOK:
                return instance.rook;
            default:
                return instance.king;
        }
    }

	public Material whiteMaterial, blackMaterial;

	private Piece PieceThatIsChecking = null;

	private List<Move> moves;

	[SerializeField]
	private float case_size = 1.3f;

	public float gameTime = 900f; //15:00

	public static float TimeToAdd = 10f;

	private Case[] cases = null;

	public GameObject[] UIimagesPieceLost;
	public Text[]		UITextsPieceLost;
	public GameObject	ChooseNewPiecePannel;

	public Camera 		MainCamera,FirstPersonCamera;
	public GameObject	FPSController;
	public Canvas		EndOfGameCanvas;
	public Text			EndOfGameText;

	private int[]		PiecesLostBySide;
	private Piece		_PawnToDestroy;

	public Case GetCaseWithIndex(int index){
		if(index < 0 || index >= 64){
			Debug.LogError("GM_GetCaseWithIndex : index not correct : " + index);
			return null;
		}
		return cases[index];}

	private static GameManager gameManager;

	private Player whitePlayer, blackPlayer;
	public bool isInit = false, isInitiating = false, gameRunning = false;

	private Player.PlayerSide actualTurn;

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

    public static Transform PieceHolder { get { return instance.pieces_holder; } private set { } }

    public static Vector3 Cimetery = new Vector3(-30,0,0);

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
		whitePlayer.Init(Player.PlayerSide.WHITE, pieces_holder, gameTime, whiteTimeText);
		blackPlayer = new GameObject("Black player").AddComponent<Player>();
		blackPlayer.Init(Player.PlayerSide.BLACK, pieces_holder, gameTime - TimeToAdd, blackTimeText);

		//remove accesibility to all Cases
		foreach(Case c in cases)
			c.setAccessibility(false);

		whitePlayer.RefreshAllPieces();
		blackPlayer.RefreshAllPieces();
        Point = 0;
		//initialise tabs
		PiecesLostBySide = new int[10]; // 10 => we don't count the king
        
        isInitiating = false;
		isInit = true;
        actualState = STATE.RUNNING;
    }
    
	public bool SaveNewPotentialMove(PotentialMove move){
		bool ret = false;
		//	Debug.Log("Potential New Move : " + m.toString());
		Piece killedPiece = move.getKilledPiece();

		if(PieceThatIsChecking != null){
			if(PieceThatIsChecking != move.getKilledPiece() && PieceThatIsChecking.CheckForCheck()){ // if this move didn't prevent the king to be killed
		//		Debug.Log(PieceThatIsChecking.toString() + " is still checking the king, should not be possible");
				move.ReverseMove();
				return true;
			}
		}

		if(killedPiece){
			if(killedPiece.getType() == Piece.PieceType.KING){
		//		Debug.Log(m.getMovedPiece().toString() + " COULD END GAME BY KILLING ENEMY KING ");
				PieceThatIsChecking = move.getMovedPiece();
				killedPiece.getActualCase().CheckCase();
				ret = true;
			}
		}
		Player enemyPlayer = move.getMovedPiece().GetPlayer().getSide() == Player.PlayerSide.WHITE ? blackPlayer : whitePlayer;
		foreach(Piece p in enemyPlayer.alivedPieces){
			if((killedPiece == null || p != killedPiece) && (p.HasThisCaseInAccessiblesOrInfluence(move.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(move.getJoinedCase()))){
				bool check = p.CheckForCheck();
				ret = ret || check;
			}
		}

		move.ReverseMove();
		if(ret){
		//	Debug.Log("POTENTIAL MOVE : " + m.toString() + " and place its king in check state !");
		}
		return ret;
	}

    public void ReverseLastMove()
    {
        Move lastMove = moves[moves.Count - 1];
        lastMove.ReverseMove(true);
        moves.Remove(lastMove);
        endOfActualTurn();
    }

    public void ApplyThisMove(Move move, bool backward)
    {
        if (!backward)
        {
            move.ApplyMove();
        }
        else
        {
            move.ReverseMove(false);
        }
    }

    public static void AddPointsForKillOf(Piece KilledPiece)
    {
        if (KilledPiece.getType() == Piece.PieceType.KING)
        {
            Debug.LogError("Can't kill the king ! ");
            return;
        }

        int side = KilledPiece.GetPlayer().getSide() == Player.PlayerSide.BLACK ? 1 : -1;
        int value = PieceValues[KilledPiece.getType()];
        Point += value * side;
    }

    public static void RemovePointsForKillOf(Piece KilledPiece)
    {
        if (KilledPiece.getType() == Piece.PieceType.KING)
        {
            Debug.LogError("Can't UNkill the king ! ");
            return;
        }

        int side = KilledPiece.GetPlayer().getSide() == Player.PlayerSide.BLACK ? 1 : -1;
        int value = PieceValues[KilledPiece.getType()];
        Point -= value * side;
    }

    public void SaveNewMove(Move move, bool saveMove = true){
        if (saveMove)
        {
            moves.Add(move);
            PanelMoves.AddMove(move);
        }
	//	Debug.Log(m.toString());
		if(PieceThatIsChecking != null && (move.getKilledPiece() == null || PieceThatIsChecking != move.getKilledPiece())){
			if(PieceThatIsChecking.CheckForCheck()){ // if this move didn't prevent the king to be killed 
				Debug.LogError("THIS MOVE SHOULD NOT HAVE POSSIBLE !");
				return;
			}else{
				Player enemyPlayer = PieceThatIsChecking.GetPlayer().getSide() == Player.PlayerSide.WHITE ? blackPlayer : whitePlayer;
				enemyPlayer.GetKing().getActualCase().UnCheckCase();
				PieceThatIsChecking = null;
			}
		}
		
		Piece killedPiece = move.getKilledPiece();
		if(killedPiece != null){ //If a Piece was destroyed by this move
			killedPiece.GetEaten();
            if (PieceThatIsChecking != null && killedPiece == PieceThatIsChecking){
				Player enemyPlayer = PieceThatIsChecking.GetPlayer().getSide() == Player.PlayerSide.WHITE ? blackPlayer : whitePlayer;
				enemyPlayer.GetKing().getActualCase().UnCheckCase();
				PieceThatIsChecking = null;
				foreach(Piece p in enemyPlayer.alivedPieces){
					p.RefreshAccessible();
				}
			}
			//ANIMATION DE DEATH ?!

			//Destroy(killedPiece.gameObject);
	
		}

		Piece movedPiece = move.getMovedPiece();
		movedPiece.RefreshAccessible();

		bool check = movedPiece.CheckForCheck();
		if(check){ // if enemy's king is checked : refresh all accessibles of its team

			Player enemyPlayer = movedPiece.GetPlayer().getSide() == Player.PlayerSide.WHITE ? blackPlayer : whitePlayer;
			foreach(Piece p in enemyPlayer.alivedPieces){
				p.RefreshAccessible();
			}
		}else{
		//	Debug.Log(movedPiece.toString() + " ENDS TURN WITHOUT SETTING OTHER KING IN CHECK MATE");
		}
		foreach(Piece p in whitePlayer.alivedPieces){
			if(p.HasThisCaseInAccessiblesOrInfluence(move.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(move.getJoinedCase()) || movedPiece.HasThisCaseInAccessiblesOrInfluence(p.getActualCase()))
			{
				p.RefreshAccessible();
			}
		}
		foreach(Piece p in blackPlayer.alivedPieces){
			if(p.HasThisCaseInAccessiblesOrInfluence(move.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(move.getJoinedCase()) || movedPiece.HasThisCaseInAccessiblesOrInfluence(p.getActualCase()))
			{
				p.RefreshAccessible();
			}
		}
        if (saveMove)
        {
            endOfActualTurn();
        }
	}

	public Move GetLastMove(){
		Move lastMove = null;
		int nbMoves = moves.Count;
		if(nbMoves > 0){
			lastMove = moves[nbMoves -1];
		}
		return lastMove;
	}

    public static void GiveUp()
    {
        instance.giveUp();
    }

	public void giveUp(){
		EndOfGame(whitePlayer.isPlaying());
	}

	public void EndOfGame(bool whiteWin){
		whitePlayer.yourTurn(false);
		blackPlayer.yourTurn(false);
		string playerWin = whiteWin ? "White" : "Black";
		EndOfGameText.text = "Player "+playerWin+" Win !";
		EndOfGameCanvas.enabled = true;
	}

	public void Rematch(){
		PiecesLostBySide = new int[10];
		for(int i =0; i< UIimagesPieceLost.Length; ++i){
			UIimagesPieceLost[i].SetActive(false);
		}
		whitePlayer.DetroyAllPieces();
		blackPlayer.DetroyAllPieces();
		Destroy(whitePlayer.gameObject);
		Destroy(blackPlayer.gameObject);
        foreach (Piece piece in pieces_holder.GetComponentsInChildren<Piece>())
        {
            Destroy(piece.gameObject);
        }
		EndOfGameCanvas.enabled = false;
		isInit = false;
		isInitiating = false;
        PanelMoves.Clear();
		Start();
	}

	void Start(){
		Init();
		beginGame();
	}

	private void endOfActualTurn(){
		Player.PlayerSide finishedTurn = actualTurn;
		if(actualTurn == Player.PlayerSide.WHITE){
			actualTurn = Player.PlayerSide.BLACK;
			
		}else{
			actualTurn = Player.PlayerSide.WHITE;
		}
		beginNewturn();
	}


	public void PlayerLostPiece(Player.PlayerSide side, Piece.PieceType type){
		int offset = side == Player.PlayerSide.WHITE ? 0 : 5;
		int index = offset + (int)type -1; //-1 => we don't count the king

		PiecesLostBySide[index]++;
		UIimagesPieceLost[index].SetActive(true);

		if(PiecesLostBySide[index] > 1){
			UITextsPieceLost[index].text = "x"+PiecesLostBySide[index];
		}
		else{
			UITextsPieceLost[index].text = "";
		}
	}

	private void beginGame(){
	 	actualTurn = Player.PlayerSide.WHITE;
		gameRunning = true;
		beginNewturn();
	}

	private void beginNewturn(){
		whitePlayer.yourTurn(actualTurn == Player.PlayerSide.WHITE);
		blackPlayer.yourTurn(actualTurn != Player.PlayerSide.WHITE);
        PanelMoves.SwitchTurn(actualTurn);
	}

	private void initPlate(){
		bool isLight = false;
		for(int i = 0; i < 64; i++){
			GameObject caseGo = isLight ? light_case : dark_case;
			Case createCase = Instantiate(caseGo, plateTransform, false).GetComponent<Case>();
			createCase.init(i, case_size);
			cases[i] = createCase;
			if((i+1) % 8 != 0)
				isLight = !isLight;
		}
	}

	public void PawnToQueen(Piece piece){
		ChooseNewPiecePannel.SetActive(true);
		_PawnToDestroy = piece;
	}

	public void TransformPawn(int pieceType){
		ChooseNewPiecePannel.SetActive(false);	

		GameObject pieceObject;
		switch ((Piece.PieceType) pieceType)
		{
			case Piece.PieceType.QUEEN :  pieceObject = queen;
				break;
			case Piece.PieceType.KNIGHT :  pieceObject = knight;
				break;
			case Piece.PieceType.BISHOP :  pieceObject = bishop;
				break;
			case Piece.PieceType.ROOK :  pieceObject = rook;
				break;
			default:
				Debug.LogError("can't choose this piece");
				pieceObject = null;
				return;
		}

		Player player = _PawnToDestroy.GetPlayer();
		Case casePawnToQueen= _PawnToDestroy.getActualCase();
		//anim to transform in something dans un ecran de fumée
		Destroy(_PawnToDestroy.gameObject);
		Piece piece = Instantiate(pieceObject, pieces_holder, false).GetComponent<Piece>();
		player.alivedPieces.Remove(_PawnToDestroy);
		casePawnToQueen.LeavePiece();
		piece.Init(player, casePawnToQueen);
		player.alivedPieces.Add(piece);
		piece.RefreshAccessible();
		Instantiate(transform_circle, casePawnToQueen.GetStandingOnPieceTransform(), false);
		_PawnToDestroy = null;

	}

	public void SelectThisCases(List<Case> casesToSelect){

	}

	void Update(){
		if(Input.GetKeyDown("space")){
			FPSController.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().ReleaseControle(FPSController.activeInHierarchy);
			FPSController.SetActive(!FPSController.activeInHierarchy);
		}
	}
}
