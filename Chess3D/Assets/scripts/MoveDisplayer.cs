using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MoveDisplayer : MonoBehaviour
{
    public enum State
    {
        LASTMOVE,
        PASSEDMOVE,
        NOTPASSEDMOVE
    }

    private bool isEven = false;
    private PanelMoves panelMoves;

    [SerializeField]
    private Text leftCase_txt, joinedCase_txt;
    [SerializeField]
    private Image MovingPiece, killedPiece;
    [SerializeField]
    private Image backgroundImage;

    public readonly Color LastColor = new Color(1f, 1f, 90f / 255f, 225f / 255f);
    public readonly Color PassedOddColor = new Color(222f / 255f, 222f / 255f, 202f / 255f, 225f / 255f);
    public readonly Color PassedEvenColor = new Color(192f / 255f, 192f / 255f, 172f / 255f, 225f / 255f);
    public readonly Color NotPassedOddColor = new Color(109f / 255f, 109f / 255f, 102f / 255f, 225f / 255f);
    public readonly Color NotPassedEvenColor = new Color(79f / 255f, 79f / 255f, 72f / 255f, 225f / 255f);

    public MoveDisplayer PreviousMove, NextMove;

    private State actualState;

    public bool IsFirstMove
    {
        get { return PreviousMove == null; }
    }

    public bool IsLastMove
    {
        get { return NextMove == null; }
    }

    public State ActualState
    {
        get { return actualState; }
        set {
            actualState = value;
            switch (actualState)
            {
                case State.LASTMOVE:
                    backgroundImage.color = LastColor;
                    break;
                case State.PASSEDMOVE:
                    backgroundImage.color = isEven ? PassedEvenColor : PassedOddColor;
                    break;
                case State.NOTPASSEDMOVE:
                    backgroundImage.color = isEven ? NotPassedEvenColor : NotPassedOddColor;
                    break;
            }
        }
    }

    public Move Move
    {
        get;
        private set;
    }

    public void Init(PanelMoves panelMoves, Move move, MoveDisplayer previousMove)
    {
        this.panelMoves = panelMoves;
        Move = move;
        PreviousMove = previousMove;
        if (IsFirstMove)
        {
            panelMoves.SetBackwardButtonInteractable(true);
            panelMoves.SetReverseButtonInteractable(true);
        }
        Image movingPieceImage = Move.getMovedPiece().Icon;
        MovingPiece.sprite = movingPieceImage.sprite;
        MovingPiece.color = movingPieceImage.color;
        leftCase_txt.text = Move.getLeftCase().generalCoordinate;
        joinedCase_txt.text = Move.getJoinedCase().generalCoordinate;
        if (Move.getKilledPiece() != null)
        {
            Image killedPieceImage = Move.getKilledPiece().Icon;
            killedPiece.sprite = killedPieceImage.sprite;
            killedPiece.color = killedPieceImage.color;
        }
        else
        {
            killedPiece.color = new Color(0f, 0f, 0f, 0f);
        }
        ActualState = State.LASTMOVE;
    }

    public void SetNextMove(MoveDisplayer nextMove)
    {
        NextMove = nextMove;
        if (NextMove != null)
        {
            ActualState = State.PASSEDMOVE;
        }
        else
        {
            ActualState = State.LASTMOVE;
            panelMoves.LastMove = this;
        }
    }
    
    public void GoToPreviousMove()
    {
        if (IsFirstMove)
        {

            panelMoves.SetBackwardButtonInteractable(false);
            //Debug.LogError("PreviousMove is Null ! You can't go BEFORE the FIRST move !");
            //return;
        }else
        {
            PreviousMove.ActualState = State.LASTMOVE;
        }

        ActualState = State.NOTPASSEDMOVE;
        panelMoves.LastMove = PreviousMove;

        if (IsLastMove)
        {
            panelMoves.SetForwardButtonInteractable(true);
            panelMoves.SetReverseButtonInteractable(false);
        }
    }

    public void GoToNextMove()
    {
        if (IsFirstMove)
        {
            panelMoves.SetBackwardButtonInteractable(true);
            //Debug.LogError("NextMove is Null ! You can't go AFTER the LAST move !");
            //return;
        }
        else
        {
            PreviousMove.ActualState = State.PASSEDMOVE;
        }
        panelMoves.LastMove = this;
        ActualState = State.LASTMOVE;

        if (IsLastMove)
        {
            panelMoves.SetForwardButtonInteractable(false);
            panelMoves.SetReverseButtonInteractable(true);
        }
    }

    public void IsReversed()
    {
        if (!IsLastMove)
        {
            Debug.LogError("This is not the LastMove ! You should be able to REVERSE this !");
            return;
        }

        if (!IsFirstMove)
        {
            PreviousMove.SetNextMove(null);
        }
        Destroy(gameObject);
    }

    public void ClearAll()
    {
        if (!IsLastMove)
        {
            NextMove.ClearAll();
        }
        else
        {
            if (!IsFirstMove)
            {
                PreviousMove.SetNextMove(null);
                PreviousMove.ClearAll();
            }
            Destroy(gameObject);
        }
    }

    public override string ToString()
    {
        return Move.toString();
    }
}
