using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece {

	[SerializeField]
	private GameObject electricBold_prefab, thunderBold_prefab;

	protected override Case getInitialCase(){
		if(player.getSide() == Player.PlayerSide.WHITE){ //white
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(2);
			else
				return GameManager.instance.GetCaseWithIndex(5);
		}else{ //black
			if(piece_type_index == 1)
				return GameManager.instance.GetCaseWithIndex(58);
			else
				return GameManager.instance.GetCaseWithIndex(61);
		}
	}

	public override bool CheckForCheck(){
		bool ret = false;
		ret = ret || checkUpLeftDiagonale();
		ret = ret || checkUpRightDiagonale();
		ret = ret || checkDownRightDiagonale();
		return ret || checkDownLeftDiagonale();
	}
	
	protected override void LookForAccessibleCases(){
		influencingCases = new List<Case>();
		List<Case> ret = getUpLeftDiagonale();
		ret.AddRange(getUpRightDiagonale());
		ret.AddRange(getDownRightDiagonale());
		ret.AddRange(getDownLeftDiagonale());
		accessibleCases = ret;
	}

	protected override IEnumerator ShortRangeAttack(Case targetCase){
		transform.LookAt(enemyPieceAttacked.transform);
		_animator.SetTrigger("ShortRangeAttack");
		yield return null;
	}

	public void CastThunderBolt(){
		if(enemyPieceAttacked != null){
			if(enemyPieceAttacked.IsSameSideAs(this)){
				Debug.LogError("Can't TRIGGER THUNDERBOLD a piece that is in the same team !");
			}else{
				Instantiate(thunderBold_prefab, enemyPieceAttacked.transform.position, Quaternion.Euler(90,0,0));
				//Debug.Break();
			}
		}else{
			Debug.LogError("EnemyPieceAttacked is null can't TRIGGER THUNDERBOLD !");
		}
	}

	public void CastElectricBolt(){
		if(enemyPieceAttacked != null){
			if(enemyPieceAttacked.IsSameSideAs(this)){
				Debug.LogError("Can't TRIGGER ELECTRICBOLD a piece that is in the same team !");
			}else{
				Instantiate(electricBold_prefab, transform, false);
				// TODO Make the bolt size change to hit perfecly enemeyPieceAttacked
			}
		}else{
			Debug.LogError("EnemyPieceAttacked is null can't TRIGGER ELECTRICBOLD !");
		}
	}

	protected override IEnumerator LongRangeAttack(Case targetCase)
	{
		transform.LookAt(enemyPieceAttacked.transform);
		_animator.SetTrigger("LongRangeAttack");
		yield return null;
	}

	public override string toString(){return player.getSide().ToString() + " BISHOP (" + piece_type_index + ")";}

	void Awake(){
		type = PieceType.BISHOP;
	}
}
