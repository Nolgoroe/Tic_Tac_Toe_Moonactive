using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerFactory
{
    //is this whole class temp??


    public static List<PlayerBase> GetPlayers(GameModeSO gameMode)
    {
        List<PlayerBase> playerList = new List<PlayerBase>(gameMode.publicModePlayers.Length);

        List<PlayerIcons> availableTypes = Enum.GetValues(typeof(PlayerIcons)).Cast<PlayerIcons>().ToList();

        foreach (PlayerData playerData in gameMode.publicModePlayers)
        {
            int randomPlayerIconIndex = UnityEngine.Random.Range(0, availableTypes.Count);

            playerList.Add(CreatePlayer(playerData.playerName, playerData, availableTypes[randomPlayerIconIndex]));

            availableTypes.RemoveAt(randomPlayerIconIndex);
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
                Debug.Log("Done here 1");

                return Resources.Load<Sprite>("ExTarget");
            case PlayerIcons.O:
                Debug.Log("Done here 2");

                return Resources.Load<Sprite>("CircleTarget");
            default:
                Debug.Log("Done here 3");

                return null;
        }
    }
}
