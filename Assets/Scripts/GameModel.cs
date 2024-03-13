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

    [Header("Game Board Data")]
    [SerializeField] Cell cellPrefab;
    [SerializeField] Transform cellsParent;
    [SerializeField] List<Cell> allEmptyGameCells;
    private Cell[,] cellsArray;

    //[Header("End conditions data")]
    //[SerializeField] int maxNumberOfCells;
    ////[SerializeField] int currentFilledCells;




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
        if(currentIndexPlayerArray >= players.Length)
        {
            currentIndexPlayerArray = 0;
        }

        currentPlayer = players[currentIndexPlayerArray];
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
            if(!cell.ReturnIsMarked())
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
                        Debug.Log("Win in row");
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
                        Debug.Log("Win in Col");
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
                    Debug.Log("Win diag 1");
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
                    Debug.Log("Win diag 2");
                    return true;
                }
            }

        }
        return false;
    }
    #endregion

    #region Return Data
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




    //Temp zone
    [ContextMenu("Test")]
    public Cell CallMiniMaxAlgo()
    {
        //this will return a cell to the AI

        int currentPlayerID = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;
        Cell[,] localCellsArray = new Cell[boardWidth, boardHeight];

        int bestMove = int.MinValue; ; // Start with the worst possible score
        Cell cellChosen = null;


        for (int i = 0; i < allEmptyGameCells.Count; i++)
        {
            localCellsArray = HardCopyCellArray(cellsArray); //current state of board 
            Cell cellToMark = allEmptyGameCells[i];

            // Assuming currentPlayerID is the AI's player ID here
            int score = minimax(currentPlayerID, cellToMark.ReturnCellCoordinatesInBoard(), localCellsArray, 9, int.MaxValue, int.MinValue);
            if (score > bestMove) // Look for a move that maximizes the score
            {
                bestMove = score;
                cellChosen = cellToMark;
            }
        }


        return cellChosen;

    }

    public int minimax(int playerID, Vector2Int cellToMarkPos, Cell[,] localCellsArray, int depth, int alpha, int beta)
    {
        localCellsArray[cellToMarkPos.x, cellToMarkPos.y].ManualMark(playerID);

        if (ReturnGeneralEndConditionMet(out EndConditions endCondition, localCellsArray, playerID) || depth == 0)
        {
            switch (endCondition)
            {
                case EndConditions.Win:
                    if (playerID == (int)currentPlayer.publicPlyerData.playerIconIndex) return 1000 + depth;
                    else return 2000;
                case EndConditions.Draw:
                    return 0;
            }

            // Reached maximum depth with no end condition met
            return 0;
        }


        int score;
        int currentPlayerID = (int)currentPlayer.publicPlyerData.playerIconIndex;
        bool isMaximizingPlayer = currentPlayerID == playerID;

        List<Cell> localEmptyCellsList = ReturnEmptyCellListFromArray(localCellsArray);

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;

            foreach (var cell in localEmptyCellsList)
            {
                Cell[,] recursiveCellArray = HardCopyCellArray(localCellsArray);
                score = minimax(ReturnNextPlayerID(playerID), cell.ReturnCellCoordinatesInBoard(), recursiveCellArray, depth - 1, alpha, beta);
                maxEval = Math.Max(maxEval, score);
                alpha = Math.Max(alpha, score);
                if (beta <= alpha)
                {
                    break; // Beta cut-off
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var cell in localEmptyCellsList)
            {
                Cell[,] recursiveCellArray = HardCopyCellArray(localCellsArray);
                score = minimax(ReturnNextPlayerID(playerID), cell.ReturnCellCoordinatesInBoard(), recursiveCellArray, depth - 1, alpha, beta);
                minEval = Math.Min(minEval, score);
                beta = Math.Min(beta, score);
                if (beta <= alpha)
                {
                    break; // Alpha cut-off
                }
            }
            return minEval;
        }
    }
    private List<Cell> ReturnEmptyCellListFromArray(Cell[,] cellArray)
    {
        List<Cell> localCellList = new List<Cell>();

        foreach (Cell cell in cellArray)
        {
            if(!cell.ReturnIsMarked())
            {
                localCellList.Add(cell);
            }
        }

        return localCellList;
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

    private Cell[,] HardCopyCellArray(Cell[,] toCopyArray)
    {
        int width = toCopyArray.GetLength(0);
        int height = toCopyArray.GetLength(1);

        Cell[,] localCellsArray = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                localCellsArray[x, y] = new Cell(toCopyArray[x,y]);
            }
        }


        return localCellsArray;
    }
}
