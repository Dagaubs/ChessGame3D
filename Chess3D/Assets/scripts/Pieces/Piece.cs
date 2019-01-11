using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {

	public enum PieceType{KING, QUEEN, BISHOP, KNIGHT, ROOK, PAWN}

	protected PieceType type;
	public PieceType getType(){return type;}

	protected Case actualCase;

	[SerializeField]
	private Renderer[] _renderers;

	protected int piece_type_index = 1;
	public void setIndex(int i){piece_type_index = i;}
	public int getIndex(){return piece_type_index;}

	protected Player player;
	public virtual Player GetPlayer(){return player;}

	public abstract string toString();

	[SerializeField]
	protected List<Case> accessibleCases = null;
	protected virtual List<Case> GetAccessibleCases(){
		if(accessibleCases == null)
			LookForAccessibleCases();
		return accessibleCases;
	}

	[SerializeField]
	protected List<Case> influencingCases = null;

	public virtual void RefreshAccessible(){
		//Debug.Log(toString() + " is refreshing is Accessibles");
		LookForAccessibleCases();
	}

	private bool dead = false;
	public bool isDead(){return dead;}

	public virtual void Init(Player p){
		player = p;
		GoTo(getInitialCase());
		gameObject.name = toString();
		if(p.getSide() == Player.PlayerSide.BLACK){
			transform.localEulerAngles = Vector3.up * 180f;
		}
		Material m = player.getSide() == Player.PlayerSide.WHITE ? GameManager.instance.whiteMaterial : GameManager.instance.blackMaterial;
		if(_renderers != null){
			foreach(Renderer render in _renderers){
				render.material = m;
			}
		}
		else{
			transform.GetChild(0).GetComponent<Renderer>().material = m;
		}
	}

	public virtual void Pick(){
		//Debug.Log("Pick is call on " + actualCase.GetIndex());
		actualCase.PickPieceOnCase();
		List<Case> casesTosetAccessible = GetAccessibleCases();
		foreach(Case c in casesTosetAccessible)
			c.setAccessibility(true);
	}

	public virtual void Unpick(){
		actualCase.UnpickPieceOnCase();
		List<Case> casesTosetAccessible = GetAccessibleCases();
		foreach(Case c in casesTosetAccessible)
			c.setAccessibility(false);
	}

	public virtual void UnpickExceptTarget(Case targetCase){
		actualCase.UnpickPieceOnCase();
		List<Case> casesTosetAccessible = GetAccessibleCases();
		foreach(Case c in casesTosetAccessible){
			if(c != targetCase)
				c.setAccessibility(false);
		}
	}

	public virtual bool HasThisCaseInAccessiblesOrInfluence(Case targetCase){
		return accessibleCases.Contains(targetCase) || influencingCases.Contains(targetCase);
	}

	protected abstract void LookForAccessibleCases();

	/*
		Test after a Potential Move if this piece check the opposite King
		return 	true if piece can destroy Opposite King
				false otherwise
	 */
	public abstract bool CheckForCheck();

	protected abstract Case getInitialCase();

	public virtual Move GoTo(Case targetCase){
		if(targetCase.isAccessible()){ // if it is legit to go to this case
			bool killedPiecebool = false;
			Piece foundPiece = null;
			if(targetCase.isTaken()){ // if there's already a piece on this target
				foundPiece = targetCase.GetStandingOnPiece();
				if(foundPiece.GetPlayer() != player){ // if it's an enemy piece
					Debug.Log(toString() + " DESTROYED A PIECE");
					killedPiecebool = true;
					//foundPiece.GetEaten();
				}else{
					Debug.LogError("Found an ally piece ! this case shouldn't be accessible !");
					targetCase.WrongCaseChoiceAnimation();
					return null;
				}
			}
			Move ret;
			if(killedPiecebool){
				ret = new Move(actualCase, targetCase, this, foundPiece);
			}
			else{
				ret = new Move(actualCase, targetCase, this);
			}

			if(actualCase != null){
				actualCase.LeavePiece();
			}
			targetCase.setAccessibility(false);
			transform.localPosition = transform.parent.InverseTransformPoint(targetCase.ComeOnPiece(this));
			actualCase = targetCase;
			//LookForAccessibleCases();
			return ret;
		}
		return null;
	}

	public void ReversePotentiallyGoTo(Case leftCase, Case joinedCase, Piece killedPiece = null){			
			if(actualCase != null){
				actualCase.LeavePiece();
			}
			if(killedPiece != null){ // if there was another piece on the joinedCase
				joinedCase.ComeOnPiece(killedPiece);
				joinedCase.setAccessibility(false);
			}
			//transform.localPosition = transform.parent.InverseTransformPoint(targetCase.ComeOnPiece(this));
			leftCase.ComeOnPiece(this);
			actualCase = leftCase;
	}

	public PotentialMove PotentiallyGoTo(Case targetCase){
		//if(targetCase.isAccessible()){ // if it is legit to go to this case
			bool killedPiecebool = false;
			Piece foundPiece = null;
			if(targetCase.isTaken()){ // if there's already a piece on this target
				foundPiece = targetCase.GetStandingOnPiece();
				if(foundPiece.GetPlayer() != player){ // if it's an enemy piece
					Debug.Log(toString() + " POTENTIALLY DESTROY A PIECE");
					killedPiecebool = true;
					//foundPiece.GetEaten();
				}else{
					Debug.LogError("Found an ally piece ! this case shouldn't be accessible !");
					targetCase.WrongCaseChoiceAnimation();
					return null;
				}
			}
			PotentialMove ret;
			if(killedPiecebool){
				ret = new PotentialMove(actualCase, targetCase, this, foundPiece);
			}
			else{
				ret = new PotentialMove(actualCase, targetCase, this);
			}

			if(actualCase != null){
				actualCase.LeavePiece();
			}
			targetCase.setAccessibility(false);
			//transform.localPosition = transform.parent.InverseTransformPoint(targetCase.ComeOnPiece(this));
			targetCase.ComeOnPiece(this);
			actualCase = targetCase;
			//LookForAccessibleCases();
			return ret;
		/*}
		return null;*/
	}

	public virtual void GetEaten(){
		// leave piece
		//actualCase.LeavePiece();
		dead = true;
		// inform player and GameManager
		player.LoseThisPiece(this);
	}

	protected bool AddIfPotentialMove(Case foundCase){
		PotentialMove newPotentialMove = PotentiallyGoTo(foundCase);
		if(newPotentialMove != null){
			return !GameManager.instance.SaveNewPotentialMove(newPotentialMove); // if this move doesn't place the king in check state (possible move)
			 // otherwise we don't add it
		}
		else{
			Debug.LogError(toString() + " - Potential Move null : case " + foundCase.GetIndex() + " was not accessible!");
			return false;
		}
	}

	protected List<Case> getUpVerticale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 7) // if we are on the top bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+8; index < 64; index=index+8){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
					return ret;
				}
			}else{
				if(AddIfPotentialMove(foundCase))
					ret.Add(foundCase);
			}
			if(index/8 == 7) // if we are on the top bounds
				return ret;
		}
		return ret;
	}

	protected List<Case> getDownVerticale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 0) // if we are on the top bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-8; index > 0; index=index-8){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
					return ret;
				}
			}else{
				if(AddIfPotentialMove(foundCase))
					ret.Add(foundCase);
			}

			if(index/8 == 0) // if we are on the top bounds
				return ret;
		}
		return ret;
	}

	protected List<Case> getLeftHorizontale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0) // if we are on the top bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-1; index > 0; index--){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
					return ret;
				}
			}
			else{
				if(AddIfPotentialMove(foundCase))
					ret.Add(foundCase);
			}
			
			if(index%8 == 0) // if we are on the top bounds
				return ret;
		}
		return ret;
	}

	protected List<Case> getRightHorizontale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7) // if we are on the right bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+1; index > 0; index++){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
					return ret;
				}
			}
			else{
				if(AddIfPotentialMove(foundCase))
					ret.Add(foundCase);
			}
			if(index%8 == 7) // if we are on the right bounds
				return ret;
		}
		return ret;
	}

	protected List<Case> getUpLeftDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 7) // if we are on the LEFT bounds OR TOP bounds
			return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+7; index < 64; index=index+7){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
					return ret;
				}
			}
			else{
				if(AddIfPotentialMove(foundCase))
					ret.Add(foundCase);
			}
			if(index%8 == 0 || index/8 == 7) // if we are on the LEFT bounds OR TOP bounds
				return ret;
		}
		return ret;
	}

	protected List<Case> getUpRightDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 7) // if we are on the RIGHT bounds OR TOP bounds
				return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+9; index < 64; index=index+9){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
					return ret;
				}
			}
			else{
				if(AddIfPotentialMove(foundCase))
					ret.Add(foundCase);
			}
			if(index%8 == 7 || index/8 == 7) // if we are on the RIGHT bounds OR TOP bounds
				return ret;
		}
		return ret;
	}

	protected List<Case> getDownRightDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 0) // if we are on the RIGHT bounds OR BOTTOM bounds
				return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-7; index > 0; index=index-7){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
					return ret;
				}
			}
			else{
				if(AddIfPotentialMove(foundCase))
					ret.Add(foundCase);
			}
			if(index%8 == 7 || index/8 == 0) // if we are on the RIGHT bounds OR BOTTOM bounds
				return ret;
		}
		return ret;
	}

	protected List<Case> getDownLeftDiagonale(){
		List<Case> ret = new List<Case>();
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 0) // if we are on the LEFT bounds OR BOTTOM bounds
				return ret;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-9; index > 0; index=index-9){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					influencingCases.Add(foundCase);
					return ret;
				}else{
					if(AddIfPotentialMove(foundCase))
						ret.Add(foundCase);
					return ret;
				}
			}
			else{
				if(AddIfPotentialMove(foundCase))
					ret.Add(foundCase);
			}
			if(index%8 == 0 || index/8 == 0) // if we are on the LEFT bounds OR BOTTOM bounds
				return ret;
		}
		return ret;
	}

	protected bool checkUpVerticale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 7) // if we are on the top bounds
			return false;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+8; index < 64; index=index+8){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return false;
				}else{ // if it's an enemy
					string piecename = foundCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
					else
						return false;
				}
			}
			
			if(index/8 == 7) // if we are on the top bounds
				return false;
		}
		return false;
	}

	protected bool checkDownVerticale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex/8 == 0) // if we are on the top bounds
			return false;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-8; index > 0; index=index-8){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return false;
				}else{ // if it's an enemy
					string piecename = foundCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
					else
						return false;
				}
			}

			if(index/8 == 0) // if we are on the top bounds
				return false;
		}
		return false;
	}

	protected bool checkLeftHorizontale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0) // if we are on the top bounds
			return false;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-1; index > 0; index--){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return false;
				}else{ // if it's an enemy
					string piecename = foundCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
					else
						return false;
				}
			}

			if(index%8 == 0) // if we are on the top bounds
				return false;
		}
		return false;
	}

	protected bool checkRightHorizontale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7) // if we are on the right bounds
			return false;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+1; index > 0; index++){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return false;
				}else{ // if it's an enemy
					string piecename = foundCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
					else
						return false;
				}
			}
			
			if(index%8 == 7) // if we are on the top bounds
				return false;
		}
		return false;
	}

	protected bool checkUpLeftDiagonale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 7) // if we are on the LEFT bounds OR TOP bounds
			return false;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+7; index < 64; index=index+7){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return false;
				}else{ // if it's an enemy
					string piecename = foundCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
					else
						return false;
				}
			}
			
			if(index%8 == 0 || index/8 == 7) // if we are on the LEFT bounds OR TOP bounds
				return false;
		}
		return false;
	}

	protected bool checkUpRightDiagonale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 7) // if we are on the RIGHT bounds OR TOP bounds
				return false;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex+9; index < 64; index=index+9){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return false;
				}else{ // if it's an enemy
					string piecename = foundCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
					else
						return false;
				}
			}
			
			if(index%8 == 7 || index/8 == 7) // if we are on the RIGHT bounds OR TOP bounds
				return false;
		}
		return false;
	}

	protected bool checkDownRightDiagonale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 7 || actualIndex/8 == 0) // if we are on the RIGHT bounds OR BOTTOM bounds
				return false;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-7; index > 0; index=index-7){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return false;
				}else{ // if it's an enemy
					string piecename = foundCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
					else
						return false;
				}
			}
			
			if(index%8 == 7 || index/8 == 0) // if we are on the RIGHT bounds OR BOTTOM bounds
				return false;
		}
		return false;
	}

	protected bool checkDownLeftDiagonale(){
		int actualIndex = actualCase.GetIndex();

		if(actualIndex%8 == 0 || actualIndex/8 == 0) // if we are on the LEFT bounds OR BOTTOM bounds
				return false;

		Case foundCase;
		GameManager gameManager = GameManager.instance;
		for(int index = actualIndex-9; index > 0; index=index-9){
			foundCase = gameManager.GetCaseWithIndex(index);
			if(foundCase.isTaken()){ //if there's another piece on the case
				if(foundCase.GetStandingOnPiece().GetPlayer() == player){// if it's an ally
					return false;
				}else{ // if it's an enemy
					string piecename = foundCase.GetStandingOnPiece().toString();
					if(piecename.Substring(piecename.Length - 4) == "KING") // if found the Enemy KING
						return true;
					else
						return false;
				}
			}

			if(index%8 == 0 || index/8 == 0) // if we are on the LEFT bounds OR BOTTOM bounds
				return false;
		}
		return false;
	}
}
