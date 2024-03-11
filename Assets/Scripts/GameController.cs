using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement; //temp
using System.Linq;

public class GameController : MonoBehaviour
{
    public static GameController Instance; //is this temp???

    [SerializeField] GameView gameViewRef;
    [SerializeField] GameModel gameModelRef;
    [SerializeField] PlayerFactory playerFactory;


    [SerializeField] GameModeSO[] allGameModes; //temp? - LOAD ALL ON STARTUP

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        InitGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void InitGame()
    {
        //Init game Model
        InitGameModel(0);

        //init game view
        InitGameView();


        //start First player turn.
        StartCoroutine(gameModelRef.ReturnCurrentPlayer().TurnStart());
    }

    private void InitGameModel(int modelID)
    {
        //Arrange needed data
        GameModeSO chosenGameMode = allGameModes[modelID]; //temp - this will be from a button later on
        PlayerBase[] players = PlayerFactory.GetPlayers(chosenGameMode).ToArray();

        //Init game model
        gameModelRef.InitGameModel(chosenGameMode, players, this);

        ConnectPlayersToEvents(players);
    }

    private void InitGameView()
    {
        gameViewRef.InitGameView();
        gameViewRef.UpdateCurrentPlayerRef(gameModelRef.ReturnCurrentPlayer());
    }


    public bool CheckEndConditions()
    {
        //Check rows
        if (gameModelRef.ReturnWinRow() || gameModelRef.ReturnWinDiagonalLeftBotRightUp() || gameModelRef.ReturnWinDiagonalRightBotLeftUp() || gameModelRef.ReturnWinColumn())
        {
            //Do win display here.

            return true;
        }

        if (gameModelRef.CheckDraw())
        {
            //handle draw here.

            return true;
        }


        return false;
    }
    public Cell ReturnRandomCell()
    {
        return gameModelRef.ReturnRandomCellInArray();
    }
    public void ConnectCellToEvents(Cell cell)
    {
        cell.OnClickOnCell += gameViewRef.UpdateCellView;
        cell.OnClickOnCell += gameModelRef.CellMarkedOnBoard;
    }
    public void ConnectPlayersToEvents(PlayerBase[] players)
    {
        //we use this here so we can more easily connect the players to all of the classes we want to have access too - such as the view AND the model

        foreach (PlayerBase player in players)
        {
            player.OnEndTurn += gameModelRef.MoveToNextPlayer;
            player.OnEndTurn += StartNextPlayerTurn;
        }
    }

    private void StartNextPlayerTurn()
    {
        gameViewRef.UpdateCurrentPlayerRef(gameModelRef.ReturnCurrentPlayer());
        StartCoroutine(gameModelRef.ReturnCurrentPlayer().TurnStart());
    }
}
