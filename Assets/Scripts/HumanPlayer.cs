using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : PlayerBase
{
    public HumanPlayer(string _playerName, Sprite _playerIconSprite, PlayerTypes _playerType, PlayerIcons _playerIcon)
        : base(_playerName, _playerIconSprite, _playerType, _playerIcon)
    {
    }

    public override IEnumerator TurnStart()
    {
        //nothing unique happens on the start of the player for now - the cells just wait for input on them.
        //the action of the "OnClickOnCell" are shot - cells get marked and then, in the model, it also checks if the controller in human or not.
        //if the controller is human - then the model (????? why model???) temp... calls the turn end of the human player.
        yield return null;
    }

    public override void TurnEnd()
    {
        Debug.Log("Human ended turn");

        if (GameController.Instance.CheckEndConditions()) return;

        OnEndTurn?.Invoke();
    }
}
