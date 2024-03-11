using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement; //temp
using System.Linq;

public class GameController : MonoBehaviour
{
    public static GameController Instance; //is this temp???
    public static bool isGameOver = false;

    [SerializeField] GameView gameViewRef;
    [SerializeField] GameModel gameModelRef;
    [SerializeField] PlayerFactory playerFactory;
    [SerializeField] UndoSystem undoSystem;


    [SerializeField] GameModeSO[] allGameModes; //temp? - LOAD ALL ON STARTUP


    [SerializeField] float currentTimerTime = 0; //temp serialized

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
    }

    private void Update()
    {
        if (isGameOver) return;

        if(Input.GetKeyDown(KeyCode.X)) //Temp
        {
            SceneManager.LoadScene(0);
        }

        if(currentTimerTime > 0)
        {
            currentTimerTime -= Time.deltaTime;

            if(currentTimerTime <= 0)
            {
                EndGameTimeout();
            }
        }
    }

    private IEnumerator InitGame(GameModeSO gameModeSO)
    {
        isGameOver = false;

        // do some view things here like animations and stuff to make the level start look cool, then after done - continue.
        // use yield return and then view functions.

        yield return null;
        //Init game Model
        InitGameModel(gameModeSO); //is it ok we pass the game mode through to here?

        //init game view
        InitGameView();

        if(!gameModelRef.ReturnCurrentGameModeSO().modelAllowUndo)
        {
            // let view remove the undo button
            gameViewRef.ToggleUndoButton(false);
        }

        //start First player turn.
        StartCoroutine(gameModelRef.ReturnCurrentPlayer().TurnStart());
    }

    private void InitGameModel(GameModeSO chosenGameMode)
    {
        //Arrange needed data
        PlayerBase[] players = PlayerFactory.GetPlayers(chosenGameMode).ToArray();

        //Init game model
        gameModelRef.InitGameModel(chosenGameMode, players, this);

        ConnectPlayersToEvents(players);

        currentTimerTime = chosenGameMode.modeTimeForTurn;
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
            isGameOver = true; //is this temp??

            return true;
        }

        if (gameModelRef.CheckDraw())
        {
            //handle draw here.
            isGameOver = true; //is this temp??

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
        cell.OnClickOnCell += undoSystem.AddCellToMarkedList;
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

    public bool ReturnCurrentPlayerIsAI()
    {
        return gameModelRef.ReturnCurrentPlayer().publicPlyerData.playerType == PlayerTypes.AI;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void StartNextPlayerTurn()
    {
        currentTimerTime = gameModelRef.ReturnCurrentGameModeSO().modeTimeForTurn;

        gameViewRef.UpdateCurrentPlayerRef(gameModelRef.ReturnCurrentPlayer());
        StartCoroutine(gameModelRef.ReturnCurrentPlayer().TurnStart());
    }

    private void EndGameTimeout()
    {
        //Handle timeput view here. - the current player loses.

        Debug.Log("Timed out");
        isGameOver = true; //is this temp??
    }

    public void SetChosenGameMode(GameModeSO gameModeSO)
    {
        // called from button
        GameModeSO chosenGameMode = gameModeSO; 

        StartCoroutine(InitGame(chosenGameMode)); //is this ok? to pass it through?
    }
}
