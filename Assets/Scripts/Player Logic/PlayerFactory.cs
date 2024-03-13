using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerFactory
{
    //This class is responsible for creating players, like in a factory.
    //Each player will be created knowing their name, connected data, randomised Icon, Sprite for Icon and type of player.

    public static List<PlayerBase> GetPlayers(GameModeSO gameMode)
    {
        List<PlayerBase> playerList = new List<PlayerBase>(gameMode.publicModePlayers.Length);

        List<PlayerIcons> availableTypes = Enum.GetValues(typeof(PlayerIcons)).Cast<PlayerIcons>().ToList();

        foreach (PlayerData playerData in gameMode.publicModePlayers)
        {
            int randomPlayerIconIndex = UnityEngine.Random.Range(0, availableTypes.Count); //randomise icon between X and O

            playerList.Add(CreatePlayer(playerData.playerName, playerData, availableTypes[randomPlayerIconIndex]));

            availableTypes.RemoveAt(randomPlayerIconIndex); //remove the icon selected so that the next player created can only be another kind of player. This also supports scaling for more Icons.
        }

        return playerList;
    }

    private static PlayerBase CreatePlayer(string name, PlayerData playerData, PlayerIcons randomisedPlayerIcon)
    {
        Sprite playerIconSprite = RetrunPlayerSprite(randomisedPlayerIcon);

        switch (playerData.playerType)
        {
            case PlayerTypes.Human:
                return new HumanPlayer(name, playerIconSprite, playerData.playerType, randomisedPlayerIcon);
            case PlayerTypes.AI:
                return new AIPlayer(name, playerIconSprite, playerData.playerType, randomisedPlayerIcon);
            default:
                return null;
        }
    }

    private static Sprite RetrunPlayerSprite(PlayerIcons playerIcon)
    {
        switch (playerIcon)
        {
            case PlayerIcons.X:
                return Resources.Load<Sprite>("ExTarget");
            case PlayerIcons.O:
                return Resources.Load<Sprite>("CircleTarget");
            default:
                Debug.Log("Error here - Could not fnid relavent sprite asset to return.");

                return null;
        }
    }
}
