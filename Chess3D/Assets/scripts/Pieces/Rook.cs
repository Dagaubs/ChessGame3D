using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece {
	
	[SerializeField]
	private float maxAttackSpeed = 6f;

	public override string toString(){return player.getSide().ToString() + " ROOK (" + piece_type_index + ")";}

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(0);
			else
				return GameManager.instance.GetCaseWithIndex(7);
		}else{ //black
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(56);
			else
				return GameManager.instance.GetCaseWithIndex(63);
		}
	}

	public override bool CheckForCheck(){
		bool ret = false;
		ret = ret || checkUpVerticale();
		ret = ret || checkDownVerticale();
		ret = ret || checkLeftHorizontale();
		return ret || checkRightHorizontale();
	}

	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
		List<Case> retUp = getUpVerticale(),
		retDown = getDownVerticale(),
		retLeft = getLeftHorizontale(),
		retRight = getRightHorizontale();
		if(retDown.Count>0)
			retUp.AddRange(retDown);

		if(retLeft.Count>0)
			retUp.AddRange(retLeft);

		if(retRight.Count>0)
			retUp.AddRange(retRight);
		accessibleCases = retUp;
	}

	void Awake(){
		type = PieceType.ROOK;
	}

	protected override IEnumerator ShortRangeAttack(Case targetCase){
		transform.LookAt(targetCase.GetStandingOnPieceTransform());

		Vector3 targetMovePosition = targetCase.GetAttackPosition(this);
		Vector3 velocity = new Vector3();
		float actualSpeed = 0f;
		_animator.SetTrigger("Attack");
		// Move To targetMovePosition
		while(Vector3.Distance(targetMovePosition, transform.position) > 0.1f){

			if(Vector3.Distance(targetMovePosition, transform.position) < 1f && actualSpeed > 2f)
				actualSpeed -= acceleration * Time.deltaTime;
			else
				actualSpeed += acceleration * Time.deltaTime;
			actualSpeed = actualSpeed > maxAttackSpeed ? maxAttackSpeed : actualSpeed;
			//_animator.SetFloat("Speed", actualSpeed);
			velocity = Vector3.forward * actualSpeed;
			transform.Translate(velocity * Time.deltaTime);
			if(Vector3.Distance(Vector3.Normalize(targetMovePosition - transform.position), transform.forward) > 0.15f){
				Debug.Log("Should be passed : " + Vector3.Normalize(targetMovePosition - transform.position) + " | forward : " + transform.forward);
				//_animator.SetFloat("Speed", 0f);
				transform.position = targetMovePosition;
			}
			yield return new WaitForFixedUpdate();
		}
		Debug.Log("Triggering Attack !");
		//_animator.SetFloat("Speed", 0f);
		_animator.SetTrigger("AttackHit");
		yield return null;
	}

}
