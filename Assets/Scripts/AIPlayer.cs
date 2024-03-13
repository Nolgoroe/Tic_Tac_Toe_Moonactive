using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : PlayerBase
{
    public AIPlayer(string _playerName, Sprite _playerIconSprite, PlayerTypes _playerType, PlayerIcons _playerIcon)
        : base(_playerName, _playerIconSprite, _playerType, _playerIcon)
    {
    }

    public override IEnumerator TurnStart()
    {
        yield return new WaitForSeconds(1f); //small delay for AI action, hardcoded


        //Choose random empty cell
        Cell localCell = GameController.Instance.ReturnAIChoice();

        //Populate that cell
        localCell.ActivateOnClickOnCellAction();


        //Check win condition in the controller
        //if (GameController.Instance.CheckEndConditions()) yield break;


        //Move to next turn
        TurnEnd();
    }

    public override void TurnEnd()
    {
        OnEndTurn?.Invoke();
    }
}
