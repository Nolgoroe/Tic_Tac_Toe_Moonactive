using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    public PlayerBase currentPlayer; //temp public??




    [SerializeField] GameView gameViewRef;
    [SerializeField] GameModel gameModelRef;
    [SerializeField] PlayerFactory playerFactory;

    [SerializeField] Cell cellPrefab;

    [SerializeField] GameModeSO[] allGameModes; //temp? - LOAD ALL ON STARTUP

    void Start()
    {
        InitGame();
    }

    private void InitGame()
    {
        //Init game Model
        InitGameModel(0);

        //init game view

        // Randomise Starting Player
    }

    private void InitGameModel(int modelID)
    {
        //Arrange needed data
        GameModeSO chosenGameMode = allGameModes[modelID]; //temp - this will be from a button later on
        PlayerBase[] players = PlayerFactory.GetPlayers(chosenGameMode).ToArray();

        //the starting player is the player with the X type - it's randomised in the factory to one of the players
        currentPlayer = players.Where(x => x.publicPlyerData.playerIcon == PlayerIcons.X).FirstOrDefault();
        if (!currentPlayer)
        {
            Debug.Log("Error finding first player!");
            return;
        }

        //Init game model
        gameModelRef.InitGameModel(chosenGameMode, players);

        gameModelRef.Test();
    }
}
