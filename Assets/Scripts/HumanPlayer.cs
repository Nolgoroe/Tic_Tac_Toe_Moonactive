using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : PlayerBase
{
    public HumanPlayer(string _playerName, Sprite _playerIconSprite, PlayerTypes _playerType, PlayerIcons _playerIcon)
        : base(_playerName, _playerIconSprite, _playerType, _playerIcon)
    {
    }
}
