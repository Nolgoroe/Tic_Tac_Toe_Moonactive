using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] int width = 3; //temp?? maybe outside information?
    [SerializeField] int height = 3; //temp?? maybe outside information?

    [SerializeField] PlayerBase currentPlayer;




    [SerializeField] GameView gameViewRef;
    [SerializeField] GameModel gameModelRef;
    [SerializeField] PlayerFactory playerFactory;

    [SerializeField] Cell cellPrefab;
    [SerializeField] Transform cellsParent;

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
        InitGameView();

        int combinedAmountOfCells = width * height;
        for (int i = 0; i < combinedAmountOfCells; i++)
        {
            Cell c = Instantiate(cellPrefab, cellsParent);
            c.OnClickOnCell += gameViewRef.UpdateCellView;
        }
    }

    private void InitGameModel(int modelID)
    {
        //Arrange needed data
        GameModeSO chosenGameMode = allGameModes[modelID]; //temp - this will be from a button later on
        PlayerBase[] players = PlayerFactory.GetPlayers(chosenGameMode).ToArray();

        //the starting player is the player with the X type - it's randomised in the factory to one of the players
        currentPlayer = players.Where(x => x.publicPlyerData.playerIconIndex == PlayerIcons.O).FirstOrDefault();

        //Init game model
        gameModelRef.InitGameModel(chosenGameMode, players);

        gameModelRef.Test();
    }

    private void InitGameView()
    {
        gameViewRef.InitGameView();
        gameViewRef.UpdateCurrentPlayerRef(currentPlayer);
    }
}
