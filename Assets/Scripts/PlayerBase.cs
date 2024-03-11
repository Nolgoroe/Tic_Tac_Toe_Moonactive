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
    public PlayerIcons playerIcon;

    [HideInInspector]
    public Sprite playerIconSprite;

}
public class PlayerBase : MonoBehaviour
{
    [SerializeField] PlayerData playerData;

    public PlayerBase(string _playerName, Sprite _playerIconSprite, PlayerTypes _playerType, PlayerIcons _playerIcon) //this whole thing... temp??
    {
        //name.... temp flag
        playerData.playerName = _playerName;
        playerData.playerType = _playerType;
        playerData.playerIcon = _playerIcon;
        playerData.playerIconSprite = _playerIconSprite;
    }




    public PlayerData publicPlyerData => playerData; //Temp
}
