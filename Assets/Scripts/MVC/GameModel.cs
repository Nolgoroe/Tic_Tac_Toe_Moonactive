using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class GameModel : MonoBehaviour
{
    #region hidden data
    [HideInInspector]
    [SerializeField] GameController gameController;

    [HideInInspector]
    [SerializeField] GameModeSO gameModeSO;
    #endregion

    [Header("Player data")]
    [SerializeField] int currentIndexPlayerArray = 0;
    private PlayerBase[] players;
    private PlayerBase currentPlayer;

    [Header("AI data")]
    [SerializeField] int chosenDepthForAlgo = -1;
    [SerializeField] int[] difficultyDepthValue;

    [Header("Game Board Data")]
    [SerializeField] Cell cellPrefab;
    [SerializeField] Transform cellsParent;
    [SerializeField] List<Cell> allEmptyGameCells;
    private Cell[,] cellsArray;

    //[Header("End conditions data")]
    //[SerializeField] int maxNumberOfCells;
    ////[SerializeField] int currentFilledCells;


    private void Start()
    {
        chosenDepthForAlgo = difficultyDepthValue[0];
    }

    #region Public Actions
    public void InitGameModel(GameModeSO _gameModeSO, PlayerBase[] _players, GameController _gameController)
    {
        gameModeSO = _gameModeSO;
        players = _players;
        gameController = _gameController;


        SpawnGridCells();
    }
    public void CellMarkedOnBoard(Cell cell)
    {
        //added as event to OnClickOnCell for cell on spawn

        cell.MarkCell(currentPlayer);
        allEmptyGameCells.Remove(cell);

        if (ReturnIsHumanControlling())
        {
            currentPlayer.TurnEnd(); //temp?? is this here?
        }

    }
    public void CellRemoveMarkOnBoard(Cell cell)
    {
        //added as event to OnRemoveCell for cell on spawn

        cell.UnMarkCell();

        allEmptyGameCells.Add(cell);
    }

    public void MoveToNextPlayer()
    {
        //added as event to OnEndTurn for players on startup
        //the reason this func is public is to allow me to centralize the addition of events on the game controller.
        //this will allow for more organised and managable code.

        currentIndexPlayerArray++;
        if (currentIndexPlayerArray >= players.Length)
        {
            currentIndexPlayerArray = 0;
        }

        currentPlayer = players[currentIndexPlayerArray];
    }

    public void SetAILevel(AILevel _chosenAILevel)
    {
        //called through controller after view adds events to buttons
        if(System.Enum.GetValues(typeof(AILevel)).Length != difficultyDepthValue.Length)
        {
            Debug.LogError("Arrays of enum and difficulty values must match in length!");
            return;
        }

        chosenDepthForAlgo = difficultyDepthValue[(int)_chosenAILevel];

    }

    #endregion

    #region End Condition Checkes

    //the logic for winning is that I check cells in a direction for the ID of the current player index.
    //if the count of consecutive ID'S reaches the count we set to win - that player won.
    //for draw, I first check all other options and then, if no one won, I check that the board is full - if it is = draw.

    public bool ReturnGeneralEndConditionMet(out EndConditions endCondition, Cell[,] _cellsArray, int playerID)
    {
        endCondition = EndConditions.None;

        if (ReturnWinRow(_cellsArray, playerID) || ReturnWinDiagonalLeftBotRightUp(_cellsArray, playerID) || ReturnWinDiagonalRightBotLeftUp(_cellsArray, playerID) || ReturnWinColumn(_cellsArray, playerID))
        {
            endCondition = EndConditions.Win;
            return true;
        }

        if (CheckDraw(_cellsArray))
        {
            endCondition = EndConditions.Draw;
            return true;
        }

        return false;

    }
    private bool CheckDraw(Cell[,] _cellsArray)
    {
        //we enter this fucntion after checking all other end conditions for a winner.

        //if we are here and there is no winner AND the board is full - we have a draw.

        foreach (Cell cell in _cellsArray)
        {
            if (!cell.ReturnIsMarked())
            {
                return false;
            }
        }
        return true;
    }

    private bool ReturnWinRow(Cell[,] _cellsArray, int playerID)
    {
        // 0,0 is top Left.

        int currentScore = 0;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        for (int row = 0; row < boardHeight; row++)
        {
            currentScore = 0;

            for (int column = 0; column < boardWidth; column++)
            {
                if (_cellsArray[column, row].ReturnMarkedIconIndex() == playerID)
                {
                    currentScore++;

                    if (currentScore == gameModeSO.modelRequiredComboToWin)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool ReturnWinColumn(Cell[,] _cellsArray, int playerID)
    {
        // 0,0 is top Left.
        int currentScore = 0;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        for (int column = 0; column < boardWidth; column++)
        {
            currentScore = 0;

            for (int row = 0; row < boardHeight; row++)
            {
                if (_cellsArray[column, row].ReturnMarkedIconIndex() == playerID)
                {
                    currentScore++;

                    if (currentScore == gameModeSO.modelRequiredComboToWin)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    private bool ReturnWinDiagonalLeftBotRightUp(Cell[,] _cellsArray, int playerID)
    {
        // 0,2 is bottom Left.

        int currentScore = 0;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        int rowOffset = boardHeight;

        for (int column = 0; column < boardWidth; column++)
        {
            rowOffset--;

            if (_cellsArray[column, rowOffset].ReturnMarkedIconIndex() == playerID)
            {
                currentScore++;

                if (currentScore == gameModeSO.modelRequiredComboToWin)
                {
                    return true;
                }
            }

        }
        return false;
    }
    private bool ReturnWinDiagonalRightBotLeftUp(Cell[,] _cellsArray, int playerID)
    {
        // 2,2 is bottom Left.

        int currentScore = 0;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        int columnOffset = boardWidth;
        int rowOffset = boardHeight;

        for (int column = columnOffset - 1; column >= 0; column--)
        {
            rowOffset--;

            if (_cellsArray[column, rowOffset].ReturnMarkedIconIndex() == playerID)
            {
                currentScore++;

                if (currentScore == gameModeSO.modelRequiredComboToWin)
                {
                    return true;
                }
            }

        }
        return false;
    }
    #endregion

    #region Public Return Data
    public PlayerBase ReturnCurrentPlayer()
    {
        return currentPlayer;
    }

    public Cell ReturnRandomCellInArray()
    {
        //used by the AI player to detect an empty cell to mark.
        // must pass through the controller to enter this function.

        Cell foundCell = null;
        List<Cell> localcells = new List<Cell>();
        localcells.AddRange(allEmptyGameCells);

        do
        {
            if (localcells.Count == 0) break;

            int randomIndex = UnityEngine.Random.Range(0, localcells.Count);

            if (localcells[randomIndex].ReturnIsMarked())
            {
                localcells.RemoveAt(randomIndex);
            }
            else
            {
                foundCell = localcells[randomIndex];
            }

        } while (foundCell == null);


        return foundCell;
    } // this might need to go away - temp - AI moved to MiniMax

    public GameModeSO ReturnCurrentGameModeSO()
    {
        return gameModeSO;
    }

    public bool ReturnIsHumanControlling()
    {
        return currentPlayer.publicPlyerData.playerType == PlayerTypes.Human;
    }

    public Cell[,] ReturnBoardCellsArray()
    {
        return cellsArray;
    }
    #endregion

    #region Private Actions
    private void SpawnGridCells()
    {
        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        cellsArray = new Cell[boardWidth, boardHeight];

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Cell cell = Instantiate(cellPrefab, cellsParent);
                cell.InitCell(x, y);
                cellsArray[x, y] = cell;
                allEmptyGameCells.Add(cell);

                gameController.ConnectCellToEvents(cell);
            }
        }

        //the starting player is the player with the X type - it's randomised in the factory.
        currentPlayer = players.Where(x => x.publicPlyerData.playerIconIndex == PlayerIcons.X).FirstOrDefault();
        currentIndexPlayerArray = System.Array.IndexOf(players, currentPlayer);
    }
    #endregion

    #region Mini Max AI Algo
    public Cell CallMiniMaxAlgo()
    {
        //this will return a cell to the AI
        int bestScore = int.MinValue;
        Cell chosenCell = null;

        int currentPlayerID = (int)currentPlayer.publicPlyerData.playerIconIndex;

        foreach (Cell cell in cellsArray)
        {
            if (!cell.ReturnIsMarked())
            {
                cell.MarkCell(currentPlayer);
                int score = minimax(cellsArray, chosenDepthForAlgo, false, currentPlayerID, int.MinValue, int.MaxValue);
                cell.UnMarkCell();


                if (score > bestScore)
                {
                    bestScore = score;
                    chosenCell = cell;
                }
            }

        }

        return chosenCell;

    }

    private int minimax(Cell[,] cellsArray, int depth, bool isMaximizing, int playerID, int alpha, int beta)
    {
        if (ReturnGeneralEndConditionMet(out EndConditions endCondition, cellsArray, playerID) || depth == 0)
        {
            return ReturnAlgoScore(endCondition, playerID, depth);
        }

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;


        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            foreach (Cell cell in cellsArray)
            {
                if (!cell.ReturnIsMarked())
                {
                    cell.MarkCell(currentPlayer);
                    int score = minimax(cellsArray, depth - 1, false, ReturnNextPlayerID(playerID), alpha, beta);
                    cell.UnMarkCell();

                    bestScore = Mathf.Max(score, bestScore);

                    alpha = Mathf.Max(alpha, score);
                    if (beta <= alpha)
                    {
                        break; // Beta cutoff
                    }

                }

            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            foreach (Cell cell in cellsArray)
            {
                if (!cell.ReturnIsMarked())
                {
                    cell.MarkCell(players[ReturnNextPlayerID(playerID)]);
                    int score = minimax(cellsArray, depth - 1, true, ReturnNextPlayerID(playerID), alpha, beta);
                    cell.UnMarkCell();

                    bestScore = Mathf.Min(score, bestScore);

                    beta = Mathf.Min(beta, score);
                    if (beta <= alpha)
                    {
                        break; // Alpha cutoff
                    }
                }

            }

            return bestScore;
        }
    }

    private int ReturnAlgoScore(EndConditions condition, int playerID, int depth)
    {
        switch (condition)
        {
            case EndConditions.Win:
                if (playerID == (int)currentPlayer.publicPlyerData.playerIconIndex) return 10 + depth;
                else
                {
                    return -10 - depth;
                }
            case EndConditions.Draw:
                return 0;
            default:
                break;
        }

        return -2;
    }

    private int ReturnNextPlayerID(int currentPlayerID)
    {
        int playerID = currentPlayerID;

        playerID++;
        if (playerID >= players.Length)
        {
            playerID = 0;
        }

        return playerID;
    }
    #endregion
}
