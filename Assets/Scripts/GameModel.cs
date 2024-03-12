using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    [SerializeField] List<Cell> allGameCells;
    private Cell[,] cellsArray;

    [Header("End conditions data")]
    [SerializeField] int maxNumberOfCells;
    [SerializeField] int currentFilledCells;




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

        currentFilledCells++;

        if(ReturnIsHumanControlling())
        {
            currentPlayer.TurnEnd(); //temp?? is this here?
        }
    }
    public void CellRemoveMarkOnBoard(Cell cell)
    {
        //added as event to OnRemoveCell for cell on spawn

        cell.UnMarkCell();

        currentFilledCells--;
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

    public bool ReturnGeneralEndConditionMet(out EndConditions endCondition)
    {
        endCondition = EndConditions.None;

        if (ReturnWinRow() || ReturnWinDiagonalLeftBotRightUp() || ReturnWinDiagonalRightBotLeftUp() || ReturnWinColumn())
        {
            endCondition = EndConditions.Win;
            return true;
        }

        if (CheckDraw())
        {
            endCondition = EndConditions.Draw;
            return true;
        }

        return false;

    }
    private bool CheckDraw()
    {
        return currentFilledCells == maxNumberOfCells;
    }

    private bool ReturnWinRow()
    {
        // 0,0 is top Left.

        int currentScore = 0;
        int currentPlayerIconIndex = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        for (int row = 0; row < boardHeight; row++)
        {
            currentScore = 0;

            for (int column = 0; column < boardWidth; column++)
            {
                if (cellsArray[column, row].ReturnMarkedIconIndex() == currentPlayerIconIndex)
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

    private bool ReturnWinColumn()
    {
        // 0,0 is top Left.
        int currentScore = 0;
        int currentPlayerIconIndex = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        for (int column = 0; column < boardWidth; column++)
        {
            currentScore = 0;

            for (int row = 0; row < boardHeight; row++)
            {
                if (cellsArray[column, row].ReturnMarkedIconIndex() == currentPlayerIconIndex)
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
    private bool ReturnWinDiagonalLeftBotRightUp()
    {
        // 0,2 is bottom Left.

        int currentScore = 0;
        int currentPlayerIconIndex = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        int rowOffset = boardHeight;

        for (int column = 0; column < boardWidth; column++)
        {
            rowOffset--;

            if (cellsArray[column, rowOffset].ReturnMarkedIconIndex() == currentPlayerIconIndex)
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
    private bool ReturnWinDiagonalRightBotLeftUp()
    {
        // 2,2 is bottom Left.

        int currentScore = 0;
        int currentPlayerIconIndex = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        int columnOffset = boardWidth;
        int rowOffset = boardHeight;

        for (int column = columnOffset - 1; column >= 0; column--)
        {
            rowOffset--;

            if (cellsArray[column, rowOffset].ReturnMarkedIconIndex() == currentPlayerIconIndex)
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
        localcells.AddRange(allGameCells);

        do
        {
            if (localcells.Count == 0) break;

            int randomIndex = Random.Range(0, localcells.Count);

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
    }

    public GameModeSO ReturnCurrentGameModeSO()
    {
        return gameModeSO;
    }

    public bool ReturnIsHumanControlling()
    {
        return currentPlayer.publicPlyerData.playerType == PlayerTypes.Human;
    }
    #endregion

    #region Private Actions


    private void SpawnGridCells()
    {
        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardHeight = (int)gameModeSO.boardWidthAndHeight.y;

        maxNumberOfCells = boardWidth * boardHeight;

        cellsArray = new Cell[boardWidth, boardHeight];

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Cell cell = Instantiate(cellPrefab, cellsParent);
                cell.InitCell(x, y);
                cellsArray[x, y] = cell;
                allGameCells.Add(cell);

                gameController.ConnectCellToEvents(cell);
            }
        }

        //the starting player is the player with the X type - it's randomised in the factory.
        currentPlayer = players.Where(x => x.publicPlyerData.playerIconIndex == PlayerIcons.X).FirstOrDefault();
        currentIndexPlayerArray = System.Array.IndexOf(players, currentPlayer);
    }
    #endregion
}
