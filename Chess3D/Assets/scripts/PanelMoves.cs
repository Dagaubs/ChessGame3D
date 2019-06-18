using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMoves : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject MoveDisplayerPrefab;
    [SerializeField]
    private Button reverseButton, backwardButton, forwardButton, giveUpButton;
    [SerializeField]
    private Transform movesContent;
    [SerializeField]
    private Image whiteStopWatchIcon, blackStopWatchIcon;
    [SerializeField]
    private Color StopWatchColorActualTurn, StopWatchColorNotYourTurn;

    //private List<MoveDisplayer> moveList;
    public MoveDisplayer LastMove, FirstMove;

    void Start()
    {
        reverseButton.onClick.AddListener(ReverseLastMove);
        reverseButton.interactable = false;
        backwardButton.onClick.AddListener(GoBackward);
        backwardButton.interactable = false;
        forwardButton.onClick.AddListener(GoForward);
        forwardButton.interactable = false;
        giveUpButton.onClick.AddListener(GiveUp);
        //moveList = new List<MoveDisplayer>();
    }

    public void AddMove(Move move)
    {
        MoveDisplayer newMove = Instantiate(MoveDisplayerPrefab, movesContent, false).GetComponent<MoveDisplayer>();
        newMove.Init(this, move, LastMove);
        //moveList.Add(newMove);
        if (LastMove != null)
        {
            LastMove.SetNextMove(newMove);
        }else
        {
            FirstMove = newMove;
        }
        LastMove = newMove;
    }

    private void ReverseLastMove()
    {
        LastMove.IsReversed();
        gameManager.ReverseLastMove();
    }

    public void SetBackwardButtonInteractable(bool state)
    {
        backwardButton.interactable = state;
    }

    public void SetForwardButtonInteractable(bool state)
    {
        forwardButton.interactable = state;
    }

    public void SetReverseButtonInteractable(bool state)
    {
        reverseButton.interactable = state;
    }

    private void GoBackward()
    {
        if (LastMove.IsLastMove)
        {
            gameManager.SetActualState(GameManager.STATE.LOOKINGBACKWARD);
        }
        //Debug.Log("LastMove.Move " + LastMove.Move.toString());
        gameManager.ApplyThisMove(LastMove.Move, true);
        LastMove.GoToPreviousMove();
    }

    private void GoForward()
    {
        //Debug.Log("Going forward : " + (LastMove == null ? "LastMove = FirstMove : " + FirstMove.ToString() : LastMove.NextMove.ToString()));
        MoveDisplayer choosingMove = LastMove == null ? FirstMove : LastMove.NextMove;
        gameManager.ApplyThisMove(choosingMove.Move, false);
        choosingMove.GoToNextMove();
        if (choosingMove.IsLastMove)
        {
            gameManager.SetActualState(GameManager.STATE.RUNNING);
        }
    }

    private void GiveUp()
    {
        GameManager.GiveUp();
    }

    public void SwitchTurn(Player.PlayerSide actualTurn)
    {
        whiteStopWatchIcon.color = actualTurn == Player.PlayerSide.WHITE ? StopWatchColorActualTurn : StopWatchColorNotYourTurn;
        blackStopWatchIcon.color = actualTurn == Player.PlayerSide.BLACK ? StopWatchColorActualTurn : StopWatchColorNotYourTurn;
    }

    public void Clear()
    {
        if (LastMove != null)
        {
            LastMove.ClearAll();
        } else if (FirstMove != null)
        {
            FirstMove.ClearAll();
        }
    }
}
