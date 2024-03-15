using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public struct PlayerData
{
    public string playerName;
    public PlayerTypes playerType;
    public bool isOnline;

    [HideInInspector]
    public PlayerIcons playerIcon;

    [HideInInspector]
    public Sprite playerIconSprite;

}
public abstract class PlayerBase
{
    public Action OnEndTurn;

    [SerializeField] private PlayerData playerData;

    
    public PlayerBase(string _playerName, Sprite _playerIconSprite, PlayerTypes _playerType, PlayerIcons _playerIcon)
    {
        playerData.playerName = _playerName;
        playerData.playerType = _playerType;
        playerData.playerIcon = _playerIcon;
        playerData.playerIconSprite = _playerIconSprite;
    }

    public abstract IEnumerator TurnStart();
    public abstract void TurnEnd();
    public PlayerData publicPlyerData => playerData;

    private void OnDisable()
    {
        OnEndTurn = null;
    }
}
