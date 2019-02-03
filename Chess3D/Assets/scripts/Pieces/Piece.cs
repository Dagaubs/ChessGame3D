using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {

	public enum PieceType{KING, QUEEN, BISHOP, KNIGHT, ROOK, PAWN}

	protected PieceType type;
	public PieceType getType(){return type;}
	
	[SerializeField]
	protected GameObject meshs, death_cloud_prefab, smokepuff_prefab;
	public void Hide(bool hide){
		meshs.SetActive(!hide);
	}

	protected Case actualCase;
	public Case getActualCase(){return actualCase;}

	protected bool _hasMoved;
	public bool HasMoved(){return _hasMoved;}

	protected Animator _animator;
	protected float maxDistanceForShortAttack = 3f;

	[SerializeField]
	private bool show = false;
	private bool saveShow = false;
	protected bool _attacking;

	void Update(){
		if(show != saveShow){
			saveShow = show;
			if(saveShow){
				Pick();
			}
			else
				Unpick();
		}
	}

	[SerializeField]
	protected float acceleration, maxSpeed;

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

	protected static Vector3 black_defaultEulerAngle = Vector3.up * 180f, white_defaultEulerAngle = Vector3.zero;
	protected Vector3 defaultEulerAngle;

	protected Piece enemyPieceAttacked = null;

	[SerializeField]
	protected List<Case> influencingCases = null;

	public virtual void RefreshAccessible(){
		//Debug.Log(toString() + " is refreshing is Accessibles");
		LookForAccessibleCases();
	}

	private bool dead = false;
	public bool isDead(){return dead;}

	public virtual void Init(Player p, Case targetCase = null){
		player = p;
		if(targetCase == null){
			targetCase = getInitialCase();
		}
		GoTo(targetCase, true, true);
		_animator = GetComponent<Animator>();
		gameObject.name = toString();
		if(p.getSide() == Player.PlayerSide.BLACK){
			defaultEulerAngle = black_defaultEulerAngle;
		}else{
			defaultEulerAngle = white_defaultEulerAngle;
		}
		transform.localEulerAngles = defaultEulerAngle;
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

	public virtual Move GoTo(Case targetCase, bool isInitiate = false, bool isSpecialMove = false){
		if(targetCase.isAccessible() || isSpecialMove){ // if it is legit to go to this case
			bool killedPiecebool = false;
			Piece foundPiece = null;
			if(targetCase.isTaken()){ // if there's already a piece on this target
				foundPiece = targetCase.GetStandingOnPiece();
				if(foundPiece.GetPlayer() != player){ // if it's an enemy piece
					//Debug.Log(toString() + " DESTROYED A PIECE");
					killedPiecebool = true;
					//foundPiece.GetEaten();
				}else{
					Debug.LogError("Found an ally piece ! this case shouldn't be accessible !");
					targetCase.WrongCaseChoiceAnimation();
					return null;
				}
			}

			int targetCaseIndex = targetCase.GetIndex();
			int actualCaseIndex =0;

			//Castling
			if(type == PieceType.KING){
				if(Mathf.Abs(targetCaseIndex - actualCaseIndex)==2){
					Piece rookForCastling = GetRookForCastling(actualCaseIndex, targetCaseIndex);
					if(rookForCastling == null){
						return null;
					}
					Case targetRookCase;
					if(targetCaseIndex > actualCaseIndex){
						targetRookCase = GameManager.instance.GetCaseWithIndex(actualCaseIndex+1);
					}
					else{
						targetRookCase = GameManager.instance.GetCaseWithIndex(actualCaseIndex+1);
					}
					rookForCastling.GoTo(targetRookCase, false, true);
				}					
			}
			else if(type == PieceType.PAWN && !killedPiecebool && actualCase != null){ // if we move in diagonal without killing anybody = prise en passant
			//	Debug.Log("actual index % 8 : " + actualCase.GetIndex() % 8 + " | target index % 8 : " + targetCase.GetIndex() % 8);
				if((actualCase.GetIndex() % 8) != (targetCase.GetIndex() % 8)){
					Case PawnToKillCase = null;
					if(player.getSide() == Player.PlayerSide.WHITE){  //prise en passant white
						PawnToKillCase = GameManager.instance.GetCaseWithIndex(targetCaseIndex-8);
					}
					else{//prise en passant black
						PawnToKillCase = GameManager.instance.GetCaseWithIndex(targetCaseIndex+8);
					}
					if(PawnToKillCase != null){
						killedPiecebool = true;
						foundPiece = PawnToKillCase.GetStandingOnPiece();
						if(foundPiece == null){
							Debug.LogError("there is no piece here");
						}
						else if(foundPiece.getType() != PieceType.PAWN){
							Debug.LogError("this is not a Pawn");
						}
					}else{
						Debug.LogError("PawnToKillCase is null");
					}
				}
			}
			Move ret;
			if(killedPiecebool){
				Debug.Log("killed someone");
				ret = new Move(actualCase, targetCase, this, foundPiece);
			}
			else{
				ret = new Move(actualCase, targetCase, this);
			}


			if(actualCase != null){
				actualCase.LeavePiece();
			}
			targetCase.setAccessibility(false);
			if(!isInitiate){
				StartCoroutine(MoveTo(targetCase, foundPiece));
				targetCase.ComeOnPiece(this);
				actualCase = targetCase;
			}
			else{
				Vector3 toDebug = targetCase.ComeOnPiece(this);
				transform.localPosition = transform.parent.InverseTransformPoint(toDebug);
				actualCase = targetCase;
			}
				
			//LookForAccessibleCases();
			return ret;
		}
		return null;
	}

	private Piece GetRookForCastling(int actualIndex, int kingTargetCaseIndex){
		int offsetFromKing;
		if(actualIndex < kingTargetCaseIndex){
			if(player.getSide() == Player.PlayerSide.WHITE){
				offsetFromKing = 3;
			}
			else{
				offsetFromKing =4;
			}
		}
		else{
			if(player.getSide() == Player.PlayerSide.WHITE){
				offsetFromKing = -4;
			}
			else{
				offsetFromKing =-3;
			}
		}

		int rookCaseIndex = actualIndex + offsetFromKing;
		Case rookCase = GameManager.instance.GetCaseWithIndex(rookCaseIndex);
		Piece rook = rookCase.GetStandingOnPiece();
		if(rook == null){
			Debug.LogError("no Piece on this case");
			return null;
		}
		else if(rook.getType() != PieceType.ROOK){
			Debug.LogError("this is not a rook");
			return null;
		}
		else if(rook.GetPlayer() != player){
			Debug.LogError("this is not my rook");
			return null;
		} 
		return rook;
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

	public bool IsSameSideAs(Piece pieceToTest){
		return player.getSide() == pieceToTest.GetPlayer().getSide();
	}

	public PotentialMove PotentiallyGoTo(Case targetCase){
		//if(targetCase.isAccessible()){ // if it is legit to go to this case
			bool killedPiecebool = false;
			Piece foundPiece = null;
			if(targetCase.isTaken()){ // if there's already a piece on this target
				foundPiece = targetCase.GetStandingOnPiece();
				if(foundPiece.GetPlayer() != player){ // if it's an enemy piece
				//	Debug.Log(toString() + " POTENTIALLY DESTROY " + foundPiece.toString() + " on " + targetCase.toString());
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

	public virtual void TriggerDeathAnimOfEnemy(string boolstring){
		if(enemyPieceAttacked != null){
			if(enemyPieceAttacked.IsSameSideAs(this)){
				Debug.LogError("Can't kill a piece that is in the same team !");
			}else{
				bool triggerSmokePuffbool = boolstring.Length == 0 || boolstring != "false";
				enemyPieceAttacked.StartDeathAnim(triggerSmokePuffbool);
			}
		}else{
			Debug.LogError("EnemyPieceAttacked is null can't call death anim !");
		}
	}

	public virtual void EndAttackAnim(){
		_attacking = false;
	}

	public virtual void StartDeathAnim(bool triggersmokePuff = true){
		if(triggersmokePuff)
			Instantiate(smokepuff_prefab, transform, false);
		_animator.SetTrigger("Death");
	}
	/*
	public virtual IEnumerator DeathAnim(){
		float timePassed = 0f, animTime = 1.75f;
		while(timePassed < animTime){
			timePassed += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
		timePassed = 0f;
		animTime = 1.75f;
		while(timePassed < animTime){
			timePassed += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
		DestroyPiece();
		yield return null;
	}
	*/
	public void InstantiateDeathCloud(){
		Instantiate(death_cloud_prefab, transform.position, Quaternion.Euler(90,0,0));
	}

	public void DestroyGameObject(){
		Destroy(gameObject);
	}

	public void DestroyPiece(){
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
			return !GameManager.instance.SaveNewPotentialMove(newPotentialMove); // if this move doesn't place the YOUR king in check state (possible move)
			 // otherwise we don't add it
		}
		else{
			Debug.LogError(toString() + " - Potential Move null : case " + foundCase.GetIndex() + " was not accessible!");
			return false;
		}
	}

	protected IEnumerator MoveTo(Case targetCase, Piece enemyPiece = null){
		if(_animator == null){
			_animator = GetComponent<Animator>();
			if(_animator == null){
				Debug.LogError("No Animator FOund ");
				yield return null;
			}
		}
		_attacking = false;
		//Piece enemyPiece = targetCase.GetStandingOnPiece();
		Vector3 targetMovePosition = transform.parent.InverseTransformPoint(targetCase.ComeOnPiece(this));
		if(enemyPiece != null) // if you have to kill a piece
		{
			_attacking = true;
			StartCoroutine(Attack(targetCase, enemyPiece));
		}

		while(_attacking) //Wait in case of Attack animation
			yield return new WaitForFixedUpdate();

		// Rotate character to make him look at the target Case
		transform.LookAt(targetCase.GetStandingOnPieceTransform());

		Vector3 velocity = new Vector3();
		float actualSpeed = 0f;
		// Move To targetMovePosition
		while(Vector3.Distance(targetMovePosition, transform.position) > 0.1f){

			if(Vector3.Distance(targetMovePosition, transform.position) < 1f && actualSpeed > 2f)
				actualSpeed -= acceleration * Time.deltaTime;
			else
				actualSpeed += acceleration * Time.deltaTime;
			actualSpeed = actualSpeed > maxSpeed ? maxSpeed : actualSpeed;
			_animator.SetFloat("Speed", actualSpeed);
			velocity = Vector3.forward * actualSpeed;
			transform.Translate(velocity * Time.deltaTime);
			if(Vector3.Distance(Vector3.Normalize(targetMovePosition - transform.position), transform.forward) > 0.1f){
				//Debug.Log("Should be passed : " + Vector3.Normalize(targetMovePosition - transform.position) + " | forward : " + transform.forward);
				_animator.SetFloat("Speed", 0f);
				transform.position = targetMovePosition;
			}
			yield return new WaitForFixedUpdate();
		}
		transform.localPosition = targetMovePosition;
		transform.localEulerAngles = defaultEulerAngle;
		_animator.SetFloat("Speed", 0f);
		yield return null;	
	}

	protected virtual IEnumerator ShortRangeAttack(Case targetCase){
		transform.LookAt(targetCase.GetStandingOnPieceTransform());

		Vector3 targetMovePosition = targetCase.GetAttackPosition(this);
		Vector3 velocity = new Vector3();
		float actualSpeed = 0f;
		// Move To targetMovePosition
		while(Vector3.Distance(targetMovePosition, transform.position) > 0.1f){

			if(Vector3.Distance(targetMovePosition, transform.position) < 1f && actualSpeed > 2f)
				actualSpeed -= acceleration * Time.deltaTime;
			else
				actualSpeed += acceleration * Time.deltaTime;
			actualSpeed = actualSpeed > maxSpeed ? maxSpeed : actualSpeed;
			_animator.SetFloat("Speed", actualSpeed);
			velocity = Vector3.forward * actualSpeed;
			transform.Translate(velocity * Time.deltaTime);
			if(Vector3.Distance(Vector3.Normalize(targetMovePosition - transform.position), transform.forward) > 0.1f){
				//Debug.Log("Should be passed : " + Vector3.Normalize(targetMovePosition - transform.position) + " | forward : " + transform.forward);
				_animator.SetFloat("Speed", 0f);
				transform.position = targetMovePosition;
			}
			yield return new WaitForFixedUpdate();
		}
		_animator.SetFloat("Speed", 0f);
		Debug.Log("triggering attack !");
		transform.LookAt(enemyPieceAttacked.transform);
		_animator.SetTrigger("Attack");
	}
	protected virtual IEnumerator LongRangeAttack(Case targetCase){
		StartCoroutine(ShortRangeAttack(targetCase));
		yield return null;
	}

	protected IEnumerator Attack(Case targetCase, Piece enemyPiece){
		Vector3 targetAttackPosition = targetCase.GetAttackPosition(this);
		enemyPieceAttacked = enemyPiece;
		if(Vector3.Distance(transform.localPosition, targetAttackPosition) > maxDistanceForShortAttack){ //Long Range Attack
			StartCoroutine(LongRangeAttack(targetCase));
		}
		else // Short Range Attack
		{
			StartCoroutine(ShortRangeAttack(targetCase));
		}
		yield return null;
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
					if(foundCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
					{
						//Debug.Log(foundCase.GetStandingOnPiece().GetPlayer().getSide().ToString() + " KING IS PLACING IN CHECK BY " + toString());
						return true;
					}
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
					if(foundCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
					{
						//Debug.Log(foundCase.GetStandingOnPiece().GetPlayer().getSide().ToString() + " KING IS PLACING IN CHECK BY " + toString());
						return true;
					}
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
					if(foundCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
					{
						//Debug.Log(foundCase.GetStandingOnPiece().GetPlayer().getSide().ToString() + " KING IS PLACING IN CHECK BY " + toString());
						return true;
					}
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
					if(foundCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
					{
						//Debug.Log(foundCase.GetStandingOnPiece().GetPlayer().getSide().ToString() + " KING IS PLACING IN CHECK BY " + toString());
						return true;
					}
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
					if(foundCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
					{
						//Debug.Log(foundCase.GetStandingOnPiece().GetPlayer().getSide().ToString() + " KING IS PLACING IN CHECK BY " + toString());
						return true;
					}
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
					if(foundCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
					{
						//Debug.Log(foundCase.GetStandingOnPiece().GetPlayer().getSide().ToString() + " KING IS PLACING IN CHECK BY " + toString());
						return true;
					}
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
					if(foundCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
					{
						//Debug.Log(foundCase.GetStandingOnPiece().GetPlayer().getSide().ToString() + " KING IS PLACING IN CHECK BY " + toString());
						return true;
					}
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
					if(foundCase.GetStandingOnPiece().getType() == Piece.PieceType.KING) // if found the Enemy KING
					{
						//Debug.Log(foundCase.GetStandingOnPiece().GetPlayer().getSide().ToString() + " KING IS PLACING IN CHECK BY " + toString());
						return true;
					}
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
