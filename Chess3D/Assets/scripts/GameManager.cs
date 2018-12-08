using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[SerializeField]
	private Transform plateTransform, pieces_holder;

	[SerializeField]
	private GameObject light_case, dark_case;

	public GameObject king, queen, rook, bishop, knight, pawn;

	public Material whiteMaterial, blackMaterial;

	private Case[] cases = null;
	public Case GetCaseWithIndex(int index){return cases[index];}

	private static GameManager gameManager;

	private Player whitePlayer, blackPlayer;
	public bool isInit = false, isInitiating = false;

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
        if (cases == null)
        {
			cases = new Case[64];
			initPlate();
        }
		//create player and make them spawn pieces
		whitePlayer = Instantiate(new GameObject()).AddComponent<Player>();
		whitePlayer.Init(Player.PlayerSide.WHITE, pieces_holder);
		blackPlayer = Instantiate(new GameObject()).AddComponent<Player>();
		blackPlayer.Init(Player.PlayerSide.BLACK, pieces_holder);

		//remove accesibility to all Cases
		foreach(Case c in cases)
			c.setAccessibility(false);

		isInitiating = false;
		isInit = true;
    }

	void Start(){
		Init();
	}

	void initPlate(){
		bool isLight = true;
		for(int i = 0; i < 64; i++){
			GameObject caseGo = isLight ? light_case : dark_case;
			Case createCase = Instantiate(caseGo, plateTransform, false).GetComponent<Case>();
			createCase.init(i);
			cases[i] = createCase;
			if((i+1) % 8 != 0)
				isLight = !isLight;
		}
	}
}
