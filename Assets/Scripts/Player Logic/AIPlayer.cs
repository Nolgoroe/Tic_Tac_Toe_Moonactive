using System.Collections;
using UnityEngine;

public class AIPlayer : PlayerBase
{
    private float timeDelayTurn = 1;
    public AIPlayer(string _playerName, Sprite _playerIconSprite, PlayerTypes _playerType, PlayerIcons _playerIcon)
        : base(_playerName, _playerIconSprite, _playerType, _playerIcon)
    {
    }
    public void SetTimeDelayTurn(float time)
    {
        timeDelayTurn = time;
    }

    public override IEnumerator TurnStart()
    {
        yield return new WaitForSeconds(timeDelayTurn); //small delay for AI action


        //Choose random empty cell
        Cell localCell = GameController.Instance.ReturnAIChoice();

        //Populate that cell
        if (localCell)
        localCell.ActivateOnClickOnCellAction();

        //Move to next turn
        TurnEnd();
    }

    public override void TurnEnd()
    {
        OnEndTurn?.Invoke();
    }

}
