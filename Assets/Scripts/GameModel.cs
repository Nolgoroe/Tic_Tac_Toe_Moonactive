using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameModel : MonoBehaviour //remove mono here // temp flag???
{
    //do we need a constructor here.


    [SerializeField] PlayerBase currentPlayer;
    [SerializeField] int currentIndexPlayerArray = 0;

    [HideInInspector]
    [SerializeField] GameController gameController;

    [HideInInspector]
    [SerializeField] GameModeSO gameModeSO;
    [SerializeField] PlayerBase[] players;

    [SerializeField] Cell cellPrefab;
    [SerializeField] Transform cellsParent;
    [SerializeField] Cell[,] cellsArray;

    [SerializeField] int maxNumberOfCells;
    [SerializeField] int currentFilledCells;

    [SerializeField] List<Cell> allGameCells;

    //I was thinking of removing the monobehavior here and make this a stricly data script.
    //I changed my mind since this script being a monobehavior allows me for easier debugging and more rapid word in the unity inspscor.
    public void InitGameModel(GameModeSO _gameModeSO, PlayerBase[] _players, GameController _gameController)
    {
        gameModeSO = _gameModeSO;
        players = _players;
        gameController = _gameController;


        SpawnGridCells();
    }

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

        //the starting player is the player with the X type - it's randomised in the factory to one of the players
        currentPlayer = players.Where(x => x.publicPlyerData.playerIconIndex == PlayerIcons.X).FirstOrDefault();
        currentIndexPlayerArray = System.Array.IndexOf(players, currentPlayer);
    }









    public bool CheckDraw()
    {
        if(currentFilledCells == maxNumberOfCells) Debug.Log("Draw"); //temp

        return currentFilledCells == maxNumberOfCells;
    }

    public bool ReturnWinRow()
    {
        // 0,0 is top Left.
        int currentScore = 0;
        int currentPlayerIconIndex = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardWHeight = (int)gameModeSO.boardWidthAndHeight.y;

        for (int row = 0; row < boardWHeight; row++)
        {
            currentScore = 0;

            for (int column = 0; column < boardWidth; column++)
            {
                if (cellsArray[column, row].ReturnMarkedIconIndex() == currentPlayerIconIndex)
                {
                    currentScore++;

                    if(currentScore == 3) // the number 3 is temp? also in all other win conditions
                    {
                        Debug.Log("Win in row");
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool ReturnWinColumn()
    {
        // 0,0 is top Left.
        int currentScore = 0;
        int currentPlayerIconIndex = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardWHeight = (int)gameModeSO.boardWidthAndHeight.y;

        for (int column = 0; column < boardWidth; column++)
        {
            currentScore = 0;

            for (int row = 0; row < boardWHeight; row++)
            {
                if (cellsArray[column, row].ReturnMarkedIconIndex() == currentPlayerIconIndex)
                {
                    currentScore++;

                    if(currentScore == 3)
                    {
                        Debug.Log("Win in Col");
                        return true;
                    }
                }
            }
        }

        return false;
    }
    public bool ReturnWinDiagonalLeftBotRightUp()
    {
        // 0,2 is bottom Left.

        int currentScore = 0;
        int currentPlayerIconIndex = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardWHeight = (int)gameModeSO.boardWidthAndHeight.y;

        int rowOffset = boardWHeight;

        for (int column = 0; column < boardWidth; column++)
        {
            rowOffset--;

            if (cellsArray[column, rowOffset].ReturnMarkedIconIndex() == currentPlayerIconIndex)
            {
                currentScore++;

                if (currentScore == 3)
                {
                    Debug.Log("Win diag 1");
                    return true;
                }
            }

        }
        return false;
    }
    public bool ReturnWinDiagonalRightBotLeftUp()
    {
        // 2,2 is bottom Left.

        int currentScore = 0;
        int currentPlayerIconIndex = (int)currentPlayer.publicPlyerData.playerIconIndex;

        int boardWidth = (int)gameModeSO.boardWidthAndHeight.x;
        int boardWHeight = (int)gameModeSO.boardWidthAndHeight.y;

        int columnOffset = boardWHeight;
        int rowOffset = boardWHeight;

        for (int column = columnOffset - 1; column >= 0; column--)
        {
            rowOffset--;

            if (cellsArray[column, rowOffset].ReturnMarkedIconIndex() == currentPlayerIconIndex)
            {
                currentScore++;

                if (currentScore == 3)
                {
                    Debug.Log("Win diag 2");
                    return true;
                }
            }

        }
        return false;
    }









    public void CellMarkedOnBoard(Cell cell)
    {
        currentFilledCells++;

        if(ReturnIsHumanControlling())
        {
            currentPlayer.TurnEnd(); //temp?? is this here?
        }
    }
    public bool ReturnIsHumanControlling()
    {
        return currentPlayer.publicPlyerData.playerType == PlayerTypes.Human;
    }

    public void MoveToNextPlayer()
    {
        currentIndexPlayerArray++;
        if(currentIndexPlayerArray >= players.Length)
        {
            currentIndexPlayerArray = 0;
        }

        currentPlayer = players[currentIndexPlayerArray];
    }
    public PlayerBase ReturnCurrentPlayer()
    {
        return currentPlayer;
    }

    public Cell ReturnRandomCellInArray()
    {
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
}
