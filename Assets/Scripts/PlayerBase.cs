using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerData
{
    public string playerName;
    public PlayerTypes playerType;
    public bool isOnline;

    [HideInInspector]
    public PlayerIcons playerIconIndex;

    [HideInInspector]
    public Sprite playerIconSprite;

}
public class PlayerBase
{
    [SerializeField] PlayerData playerData;

    public PlayerBase(string _playerName, Sprite _playerIconSprite, PlayerTypes _playerType, PlayerIcons _playerIcon) //this whole thing... temp??
    {
        playerData.playerName = _playerName;
        playerData.playerType = _playerType;
        playerData.playerIconIndex = _playerIcon;
        playerData.playerIconSprite = _playerIconSprite;
    }




    public PlayerData publicPlyerData => playerData; //Temp
}
