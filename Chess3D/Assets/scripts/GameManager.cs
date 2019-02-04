using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

	[SerializeField]
	private Transform plateTransform, pieces_holder;

	[SerializeField]
	private GameObject light_case, dark_case, transform_circle;

	[SerializeField]
	private Text whiteTimeText, blackTimeText;

	public GameObject king, queen, rook, bishop, knight, pawn;

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

		//initialise tabs
		PiecesLostBySide = new int[10]; // 10 => we don't count the king

		isInitiating = false;
		isInit = true;
    }

	public bool SaveNewPotentialMove(PotentialMove m){
		bool ret = false;
		//	Debug.Log("Potential New Move : " + m.toString());
		Piece killedPiece = m.getKilledPiece();

		if(PieceThatIsChecking != null){
			if(PieceThatIsChecking != m.getKilledPiece() && PieceThatIsChecking.CheckForCheck()){ // if this move didn't prevent the king to be killed
		//		Debug.Log(PieceThatIsChecking.toString() + " is still checking the king, should not be possible");
				m.ReverseMove();
				return true;
			}
		}
		bool usefulToTestJoinedCase = true;
		if(killedPiece){
			if(killedPiece.getType() == Piece.PieceType.KING){
		//		Debug.Log(m.getMovedPiece().toString() + " COULD END GAME BY KILLING ENEMY KING ");
				PieceThatIsChecking = m.getMovedPiece();
				ret = true;
			}
			else if(killedPiece == PieceThatIsChecking){
		//		Debug.Log(m.getMovedPiece().toString() + " COULD KILL THE PIECE THAT IS CHECKING KING ");
				usefulToTestJoinedCase = false;
			}
		}
		Player enemyPlayer = m.getMovedPiece().GetPlayer().getSide() == Player.PlayerSide.WHITE ? blackPlayer : whitePlayer;
		foreach(Piece p in enemyPlayer.alivedPieces){
			if(p != killedPiece && (p.HasThisCaseInAccessiblesOrInfluence(m.getLeftCase()) || (usefulToTestJoinedCase && p.HasThisCaseInAccessiblesOrInfluence(m.getJoinedCase())))){
				bool check = p.CheckForCheck();
				ret = ret || check;
			}
		}

		m.ReverseMove();
		if(ret){
		//	Debug.Log("POTENTIAL MOVE : " + m.toString() + " and place its king in check state !");
		}
		return ret;
	}

	public void SaveNewMove(Move m){
		moves.Add(m);
	//	Debug.Log(m.toString());
		if(PieceThatIsChecking != null && (m.getKilledPiece() == null || PieceThatIsChecking != m.getKilledPiece())){
			if(PieceThatIsChecking.CheckForCheck()){ // if this move didn't prevent the king to be killed 
				Debug.LogError("THIS MOVE SHOULD NOT HAVE POSSIBLE !");
				return;
			}else{
				//Player enemyPlayer = PieceThatIsChecking.GetPlayer().getSide() == Player.PlayerSide.WHITE ? blackPlayer : whitePlayer;
				PieceThatIsChecking = null;
			}
		}
		
		Piece killedPiece = m.getKilledPiece();
		if(killedPiece != null){ //If a Piece was destroyed by this move
			killedPiece.GetEaten();
			if(PieceThatIsChecking != null && killedPiece == PieceThatIsChecking){
				PieceThatIsChecking = null;
				foreach(Piece p in m.getMovedPiece().GetPlayer().alivedPieces){
					p.RefreshAccessible();
				}
			}
			//ANIMATION DE DEATH ?!

			//Destroy(killedPiece.gameObject);
	
		}

		Piece movedPiece = m.getMovedPiece();
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
			if(p.HasThisCaseInAccessiblesOrInfluence(m.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(m.getJoinedCase()) || movedPiece.HasThisCaseInAccessiblesOrInfluence(p.getActualCase()))
			{
				p.RefreshAccessible();
			}
		}
		foreach(Piece p in blackPlayer.alivedPieces){
			if(p.HasThisCaseInAccessiblesOrInfluence(m.getLeftCase()) || p.HasThisCaseInAccessiblesOrInfluence(m.getJoinedCase()) || movedPiece.HasThisCaseInAccessiblesOrInfluence(p.getActualCase()))
			{
				p.RefreshAccessible();
			}
		}
		endOfActualTurn();
	}

	public Move GetLastMove(){
		Move lastMove = null;
		int nbMoves = moves.Count;
		if(nbMoves > 0){
			lastMove = moves[nbMoves -1];
		}
		return lastMove;
	}

	public void GiveUp(){
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
		EndOfGameCanvas.enabled = false;
		isInit = false;
		isInitiating = false;
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
		Instantiate(transform_circle, casePawnToQueen.GetStandingOnPieceTransform(), false);
		_PawnToDestroy = null;

	}

	public void SelectThisCases(List<Case> casesToSelect){

	}

	void Update(){
		bool FPS = MainCamera.gameObject.activeInHierarchy;
		if(Input.GetKeyDown("space")){
			MainCamera.gameObject.SetActive(!FPS);
			FirstPersonCamera.gameObject.SetActive(FPS);
		}
	}
}
