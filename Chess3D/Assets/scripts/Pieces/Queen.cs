using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{

    [SerializeField]
    private GameObject meteor_prefab, fireCircle_prefab;

    public override PieceType getType()
    {
        return PieceType.QUEEN;
    }

    public override string toString() { return player.getSide().ToString() + " QUEEN"; }

    protected override Case getInitialCase()
    {
        if (player.getSide() == Player.PlayerSide.WHITE)
        { //white
            return GameManager.instance.GetCaseWithIndex(3);
        }
        else { //black
            return GameManager.instance.GetCaseWithIndex(59);
        }
    }

    public override bool CheckForCheck()
    {
        bool ret = false;
        ret = ret || checkUpLeftDiagonale();
        ret = ret || checkUpRightDiagonale();
        ret = ret || checkDownRightDiagonale();
        ret = ret || checkDownLeftDiagonale();
        ret = ret || checkUpVerticale();
        ret = ret || checkDownVerticale();
        ret = ret || checkLeftHorizontale();
        ret = ret || checkRightHorizontale();
        return ret;
    }

    protected override void LookForAccessibleCases()
    {
        influencingCases = new List<Case>();
        List<Case> ret = getUpLeftDiagonale();
        ret.AddRange(getUpRightDiagonale());
        ret.AddRange(getDownRightDiagonale());
        ret.AddRange(getDownLeftDiagonale());
        ret.AddRange(getUpVerticale());
        ret.AddRange(getDownVerticale());
        ret.AddRange(getLeftHorizontale());
        ret.AddRange(getRightHorizontale());
        accessibleCases = ret;
    }

    public void CastMeteor()
    {
        if (enemyPieceAttacked != null)
        {
            if (enemyPieceAttacked.IsSameSideAs(this))
            {
                Debug.LogError("Can't TRIGGER METEOR a piece that is in the same team !");
            }
            else {
                Meteor meteor = Instantiate(meteor_prefab, enemyPieceAttacked.transform.position + Vector3.up * 7f, Quaternion.Euler(-110, 0, 0)).GetComponent<Meteor>();
                meteor.Init(enemyPieceAttacked);
                //Debug.Break();
            }
        }
        else {
            Debug.LogError("EnemyPieceAttacked is null can't TRIGGER METEOR !");
        }
    }

    public void CastFireCircle()
    {
        if (enemyPieceAttacked != null)
        {
            if (enemyPieceAttacked.IsSameSideAs(this))
            {
                Debug.LogError("Can't TRIGGER METEOR a piece that is in the same team !");
            }
            else {
                Instantiate(fireCircle_prefab, enemyPieceAttacked.transform.position, Quaternion.Euler(90, 0, 0));
            }
        }
        else {
            Debug.LogError("EnemyPieceAttacked is null can't TRIGGER METEOR !");
        }
    }

    protected override IEnumerator ShortRangeAttack(Case targetCase)
    {
        if (!animationStopped)
        {
            transform.LookAt(enemyPieceAttacked.transform);
            _animator.SetTrigger("ShortRangeAttack");
        }
        yield return null;
    }

    protected override IEnumerator LongRangeAttack(Case targetCase)
    {
        if (!animationStopped)
        {
            transform.LookAt(enemyPieceAttacked.transform);
            _animator.SetTrigger("LongRangeAttack");
        }
        yield return null;
    }

}
