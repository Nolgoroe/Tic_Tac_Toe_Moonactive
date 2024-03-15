using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerFactory
{
    //This class is responsible for creating players, like in a factory.
    //Each player will be created knowing their name, connected data, randomised Icon, Sprite for Icon and type of player.

    public static List<PlayerBase> GetPlayers(GameModeSO gameMode)
    {
        List<PlayerBase> playerList = new List<PlayerBase>(gameMode.modePublicModePlayers.Length);

        List<PlayerIcons> availableTypes = Enum.GetValues(typeof(PlayerIcons)).Cast<PlayerIcons>().ToList();

        foreach (PlayerData playerData in gameMode.modePublicModePlayers)
        {
            int randomPlayerIconIndex = UnityEngine.Random.Range(0, availableTypes.Count); //randomise icon between X and O

            playerList.Add(CreatePlayer(playerData.playerName, playerData, availableTypes[randomPlayerIconIndex], gameMode.modeTimeDelayAITurn));

            availableTypes.RemoveAt(randomPlayerIconIndex); //remove the icon selected so that the next player created can only be another kind of player. This also supports scaling for more Icons.
        }

        return playerList;
    }

    private static PlayerBase CreatePlayer(string name, PlayerData playerData, PlayerIcons randomisedPlayerIcon, float AITimeDelay)
    {
        Sprite playerIconSprite = RetrunPlayerSprite(randomisedPlayerIcon);

        switch (playerData.playerType)
        {
            case PlayerTypes.Human:
                return new HumanPlayer(name, playerIconSprite, playerData.playerType, randomisedPlayerIcon);
            case PlayerTypes.AI:
                AIPlayer aiPlayer = new AIPlayer(name, playerIconSprite, playerData.playerType, randomisedPlayerIcon);
                aiPlayer.SetTimeDelayTurn(AITimeDelay);
                return aiPlayer;
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
