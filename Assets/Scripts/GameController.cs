using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //temp

public class GameController : MonoBehaviour
{
    public static GameController Instance; //is this temp???
    public static bool isGameOver = false;

    [Header("Required References")]
    [SerializeField] GameView gameViewRef;
    [SerializeField] GameModel gameModelRef;
    [SerializeField] PlayerFactory playerFactory;
    [SerializeField] UndoSystem undoSystem;

    [Header("Turn Timer Data")]
    [SerializeField] float currentTimerTime = 0; //is it ok for the timer to be on the controller? temp??

    [Header("AI Player TEMP")]
    [SerializeField] AILevel aiLevel;


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

    private void Update()
    {
        if (isGameOver) return;

        if(currentTimerTime > 0)
        {
            currentTimerTime -= Time.deltaTime;

            gameViewRef.UpdateTurnTimer(currentTimerTime);
            if (currentTimerTime <= 0)
            {
                EndGameTimeout();
            }
        }
    }

    #region Init Funcitons
    private IEnumerator InitGame(GameModeSO gameModeSO)
    {
        //Set default game data
        isGameOver = false;
        currentTimerTime = gameModeSO.modeTimeForTurn;

        // do some view things here like animations and stuff to make the level start look cool, then after done - continue.
        // use yield return and then view functions.

        yield return null;
        //Init game Model
        InitGameModel(gameModeSO); //is it ok we pass the game mode through to here?

        //init game view
        InitGameView();

        //allow undo only if the gamemodeSO allows it.
        if(!gameModelRef.ReturnCurrentGameModeSO().modelAllowUndo)
        {
            // let view remove the undo button
            gameViewRef.TogglePVCButtons(false);
        }

        //start First player turn - this starts the game.
        StartCoroutine(gameModelRef.ReturnCurrentPlayer().TurnStart());
    }

    private void InitGameModel(GameModeSO chosenGameMode)
    {
        //Create the game mode players in the factory by the player types in the gamemodeSO
        //This will just return the relavent classes of our players
        PlayerBase[] players = PlayerFactory.GetPlayers(chosenGameMode).ToArray();

        //Init game model
        gameModelRef.InitGameModel(chosenGameMode, players, this);

        ConnectPlayersToEvents(players);
    }

    private void InitGameView()
    {
        gameViewRef.InitGameView();
        gameViewRef.UpdatePlayerView(gameModelRef.ReturnCurrentPlayer()); //temp
    }
    #endregion

    #region Private Actions
    private void ConnectPlayersToEvents(PlayerBase[] players)
    {
        //we use this here so we can more easily connect the players to all of the classes we want to have access too - such as the view AND the model

        foreach (PlayerBase player in players)
        {
            player.OnEndTurn += CheckEndConditions;
            player.OnEndTurn += gameModelRef.MoveToNextPlayer;
            player.OnEndTurn += StartNextPlayerTurn;
        }
    }

    private void StartNextPlayerTurn()
    {
        currentTimerTime = gameModelRef.ReturnCurrentGameModeSO().modeTimeForTurn;

        gameViewRef.UpdatePlayerView(gameModelRef.ReturnCurrentPlayer()); //temp
        StartCoroutine(gameModelRef.ReturnCurrentPlayer().TurnStart());
    }

    private void EndGameTimeout()
    {
        Debug.Log("Timed out");

        SetGameOver(EndConditions.Timeout);
    }

    private void SetGameOver(EndConditions endCondition)
    {
        PlayerBase currentPlayer = gameModelRef.ReturnCurrentPlayer();
        isGameOver = true; //is this temp??

        gameViewRef.SetEndScreenText(endCondition, currentPlayer);
        //this is where we pass the end condition and player to the view and it manages what to show in a switch.
    }
    #endregion

    #region Public Actions
    public void CheckEndConditions()
    {
        //called from both the AI and Human players after they mark a cell.

        int currentPlayerID = (int)gameModelRef.ReturnCurrentPlayer().publicPlyerData.playerIconIndex;
        Cell[,] boardCell = gameModelRef.ReturnBoardCellsArray();

        if (gameModelRef.ReturnGeneralEndConditionMet(out EndConditions endCondition, boardCell, currentPlayerID))
        {
            SetGameOver(endCondition);
            //return true;
        }

        //return false;
    }
    public void ConnectCellToEvents(Cell cell)
    {
        cell.OnClickOnCell += gameModelRef.CellMarkedOnBoard;
        cell.OnClickOnCell += gameViewRef.AnimateCellMark;
        cell.OnClickOnCell += undoSystem.AddCellToMarkedList;

        cell.OnRemoveCell += gameModelRef.CellRemoveMarkOnBoard;
    }
    #endregion

    #region Public Return Data
    public Cell ReturnRandomCell()
    {
        //Used by AI to find cell to mark
        return gameModelRef.ReturnRandomCellInArray();
    }
    public Cell ReturnAIChoice()
    {
        //Used by AI to find cell to mark
        return gameModelRef.CallMiniMaxAlgo();
    }
    public bool ReturnCurrentPlayerIsHuman()
    {
        return gameModelRef.ReturnIsHumanControlling();
    }
    #endregion

    #region Connected To Buttons
    public void SetChosenGameMode(GameModeSO gameModeSO)
    {        
        // called from button

        if (gameModelRef.ReturnCurrentGameModeSO()) return; //prevent multiple start games

        GameModeSO chosenGameMode = gameModeSO; 

        StartCoroutine(InitGame(chosenGameMode)); //is this ok? to pass it through?
    }
    public void RestartGame()
    {
        //called from button
        SceneManager.LoadScene(0);
    }

    public void Hint()
    {
        //called from button

        Cell cell = ReturnRandomCell();
        cell.SetAsHint();
    }
    #endregion











    
}
