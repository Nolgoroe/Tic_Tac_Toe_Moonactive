using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance; 
    public static bool isGameOver = false;

    [Header("Required References")]
    [SerializeField] GameView gameViewRef;
    [SerializeField] GameModel gameModelRef;
    [SerializeField] PlayerFactory playerFactory;
    [SerializeField] UndoSystem undoSystem;

    [Header("Turn Timer Data")]
    [SerializeField] float currentTimerTime = 0;
    [SerializeField] float timeForTurn = 5;


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

    private void Start()
    {
        gameViewRef.InitGameView(this);
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
        currentTimerTime = timeForTurn;

        // do some view things here like animations and stuff to make the level start look cool, then after done - continue.
        // use yield return and then view functions.

        yield return null;
        //Init game Model
        GameModelStartup(gameModeSO);

        //init game view
        ViewStartup();

        //allow undo only if the gamemodeSO allows it.
        if(!gameModelRef.ReturnCurrentGameModeSO().modeAllowUndo)
        {
            // let view remove the undo button
            gameViewRef.TogglePVCButtons(false);
        }

        //start First player turn - this starts the game.
        StartCoroutine(gameModelRef.ReturnCurrentPlayer().TurnStart());
    }

    private void GameModelStartup(GameModeSO chosenGameMode)
    {
        //Create the game mode players in the factory by the player types in the gamemodeSO
        //This will just return the relavent classes of our players
        PlayerBase[] players = PlayerFactory.GetPlayers(chosenGameMode).ToArray();

        //Init game model
        gameModelRef.InitGameModel(chosenGameMode, players, this);

        ConnectPlayersToEvents(players);
    }

    private void ViewStartup()
    {
        gameViewRef.GameViewStartup();
        gameViewRef.UpdatePlayerView(gameModelRef.ReturnCurrentPlayer());
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
        if (isGameOver) return;
        currentTimerTime = timeForTurn;

        gameViewRef.UpdatePlayerView(gameModelRef.ReturnCurrentPlayer());
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
        isGameOver = true;

        //this is where we pass the end condition and player to the view and it manages what to show in a switch.
        gameViewRef.SetEndScreenText(endCondition, currentPlayer);
    }

    private Cell ReturnRandomCell()
    {
        //used to show Hint
        return gameModelRef.ReturnRandomCellInArray();
    }
    #endregion

    #region Public Actions
    public void CheckEndConditions()
    {
        //called from both the AI and Human players after they mark a cell.

        Cell[,] boardCell = gameModelRef.ReturnBoardCellsArray();


        if (gameModelRef.ReturnGeneralEndConditionMet(out EndConditions endCondition, boardCell, gameModelRef.ReturnCurrentPlayer(), out PlayerIcons winningPlayer))
        {
            SetGameOver(endCondition);
        }
    }
    public void ConnectCellToEvents(Cell cell)
    {
        cell.OnClickOnCell += gameModelRef.CellMarkedOnBoard;
        cell.OnClickOnCell += gameViewRef.AnimateCellMark;
        cell.OnClickOnCell += undoSystem.AddCellToMarkedList;

        cell.OnRemoveCell += gameModelRef.CellRemoveMarkOnBoard;
    }

    public void ManualEndTurnPlayer(PlayerBase player)
    {
        player.TurnEnd();
    }
    #endregion

    #region Public Return Data
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

    #region Related To Buttons
    public void SetChosenGameMode(GameModeSO gameModeSO)
    {        
        // called from button

        if (gameModelRef.ReturnCurrentGameModeSO()) return; //prevent multiple start games

        GameModeSO chosenGameMode = gameModeSO; 

        StartCoroutine(InitGame(chosenGameMode));
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
    public void SetAILevel(AILevel aiLevel)
    {
        //connected through view to buttons
        gameModelRef.SetAILevel(aiLevel);
    }
    public void SetTimeForTurn(int value)
    {
        timeForTurn = value;
    }
    #endregion  
}
