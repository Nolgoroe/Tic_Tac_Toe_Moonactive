using System.Collections;
using UnityEngine;

public class HumanPlayer : PlayerBase
{
    public HumanPlayer(string _playerName, Sprite _playerIconSprite, PlayerTypes _playerType, PlayerIcons _playerIcon)
        : base(_playerName, _playerIconSprite, _playerType, _playerIcon)
    {
    }

    public override IEnumerator TurnStart()
    {
        //nothing unique happens on the start of the player - the cells just wait for input on them.
        //the action of the "OnClickOnCell" are shot - cells get marked and then, in the model, it also checks if the controller in human or not.
        //if the controller is human - then the model calls the turn end of the human player on the game controller.
        yield return null;
    }

    public override void TurnEnd()
    {
        OnEndTurn?.Invoke();
    }
}
