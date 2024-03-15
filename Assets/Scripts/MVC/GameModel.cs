using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameModel : MonoBehaviour
{
    const int EMPTY_CELL_INDEX = -1;

    #region hidden data
    [HideInInspector]
    [SerializeField] GameController gameController;
    #endregion

    [Header("Game Mode data")]
    [SerializeField] GameModeSO chosenGameModeSO; //also used as the default game mode for unit tests
    [SerializeField] GameModeSO testGameMode;

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

    private void Start()
    {
        chosenDepthForAlgo = difficultyDepthValue[0];
    }

    #region Public Actions
    public void InitGameModel(GameModeSO _gameModeSO, PlayerBase[] _players, GameController _gameController)
    {
        chosenGameModeSO = _gameModeSO;
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
            gameController.ManualEndTurnPlayer(currentPlayer);
        }

    }
    public void CellRemoveMarkOnBoard(Cell cell)
    {
        //added as event to OnRemoveCell for cell on spawn

        cell.UnMarkCell(EMPTY_CELL_INDEX);

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

    public void UnitTestSetGameMode()
    {
        chosenGameModeSO = testGameMode;
    }
    #endregion

    #region End Condition Checkes

    //the logic for winning is that I check cells in a direction for the ID of the current player index.
    //if the count of consecutive ID'S reaches the count we set to win - that player won.
    //for draw, I first check all other options and then, if no one won, I check that the board is full - if it is = draw.

    public bool ReturnGeneralEndConditionMet(out EndConditions endCondition, int[,] _cellsArray, PlayerBase player, out PlayerIcons winningPlayerIcon)
    {
        endCondition = EndConditions.None;
        PlayerIcons icon = player.publicPlyerData.playerIcon;

        if (ReturnWinRow(_cellsArray, icon, out winningPlayerIcon) || 
            ReturnWinDiagonalLeftBotRightUp(_cellsArray, icon, out winningPlayerIcon) || 
            ReturnWinDiagonalRightBotLeftUp(_cellsArray, icon, out winningPlayerIcon) || 
            ReturnWinColumn(_cellsArray, icon, out winningPlayerIcon))
        {
            endCondition = EndConditions.End;
            return true;
        }

        if (CheckDraw(_cellsArray))
        {
            endCondition = EndConditions.Draw;
            return true;
        }

        return false;

    }
    private bool CheckDraw(int[,] _cellsArray)
    {
        //we enter this fucntion after checking all other end conditions for a winner.

        //if we are here and there is no winner AND the board is full - we have a draw.

        foreach (int cellIndex in _cellsArray)
        {
            if (cellIndex == EMPTY_CELL_INDEX)
            {
                return false;
            }
        }
        return true;
    }

    private bool ReturnWinRow(int[,] _cellsArray, PlayerIcons playerIcon, out PlayerIcons winningPlayer)
    {
        // 0,0 is top Left.
        winningPlayer = PlayerIcons.O;

        int boardWidth = (int)chosenGameModeSO.modeBoardWidthAndHeight.x;
        int boardHeight = (int)chosenGameModeSO.modeBoardWidthAndHeight.y;

        for (int row = 0; row < boardHeight; row++)
        {
            int currentPlayerScore = 0;
            int enemyPlayerScrore = 0;

            for (int column = 0; column < boardWidth; column++)
            {
                if (_cellsArray[column, row] == (int)playerIcon)
                {
                    currentPlayerScore++;
                    enemyPlayerScrore = 0;

                    if (currentPlayerScore == chosenGameModeSO.modeRequiredComboToWin)
                    {
                        winningPlayer = playerIcon;

                        return true;
                    }
                }
                else
                {
                    if (_cellsArray[column, row] != EMPTY_CELL_INDEX)
                    {
                        currentPlayerScore = 0;
                        enemyPlayerScrore++;

                        if (enemyPlayerScrore == chosenGameModeSO.modeRequiredComboToWin)
                        {
                            winningPlayer = ReturnOtherPlayerIcon(playerIcon);

                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private bool ReturnWinColumn(int[,] _cellsArray, PlayerIcons playerIcon, out PlayerIcons winningPlayer)
    {
        // 0,0 is top Left.
        winningPlayer = PlayerIcons.O;

        int boardWidth = (int)chosenGameModeSO.modeBoardWidthAndHeight.x;
        int boardHeight = (int)chosenGameModeSO.modeBoardWidthAndHeight.y;

        for (int column = 0; column < boardWidth; column++)
        {
            int currentPlayerScore = 0;
            int enemyPlayerScrore = 0;

            for (int row = 0; row < boardHeight; row++)
            {
                if (_cellsArray[column, row] == (int)playerIcon)
                {
                    currentPlayerScore++;
                    enemyPlayerScrore = 0;

                    if (currentPlayerScore == chosenGameModeSO.modeRequiredComboToWin)
                    {
                        winningPlayer = playerIcon;
                        return true;
                    }
                }
                else
                {
                    if(_cellsArray[column, row] != EMPTY_CELL_INDEX)
                    {
                        currentPlayerScore = 0;
                        enemyPlayerScrore++;

                        if (enemyPlayerScrore == chosenGameModeSO.modeRequiredComboToWin)
                        {
                            winningPlayer = ReturnOtherPlayerIcon(playerIcon);

                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
    private bool ReturnWinDiagonalLeftBotRightUp(int[,] _cellsArray, PlayerIcons playerIcon, out PlayerIcons winningPlayer)
    {
        // 0,2 is bottom Left.
        winningPlayer = PlayerIcons.O;

        int currentPlayerScore = 0;
        int enemyPlayerScrore = 0;

        int boardWidth = (int)chosenGameModeSO.modeBoardWidthAndHeight.x;
        int boardHeight = (int)chosenGameModeSO.modeBoardWidthAndHeight.y;

        int rowOffset = boardHeight;

        for (int column = 0; column < boardWidth; column++)
        {
            rowOffset--;

            if (_cellsArray[column, rowOffset] == (int)playerIcon)
            {
                currentPlayerScore++;
                enemyPlayerScrore = 0;

                if (currentPlayerScore == chosenGameModeSO.modeRequiredComboToWin)
                {
                    winningPlayer = playerIcon;
                    return true;
                }
            }
            else
            {
                if (_cellsArray[column, rowOffset] != EMPTY_CELL_INDEX)
                {
                    currentPlayerScore = 0;
                    enemyPlayerScrore++;

                    if (enemyPlayerScrore == chosenGameModeSO.modeRequiredComboToWin)
                    {
                        winningPlayer = ReturnOtherPlayerIcon(playerIcon);

                        return true;
                    }
                }
            }

        }
        return false;
    }
    private bool ReturnWinDiagonalRightBotLeftUp(int[,] _cellsArray,  PlayerIcons playerIcon, out PlayerIcons winningPlayer)
    {
        // 2,2 is bottom Right.
        winningPlayer = PlayerIcons.O;

        int currentPlayerScore = 0;
        int enemyPlayerScrore = 0;

        int boardWidth = (int)chosenGameModeSO.modeBoardWidthAndHeight.x;
        int boardHeight = (int)chosenGameModeSO.modeBoardWidthAndHeight.y;

        int rowOffset = boardHeight;

        for (int column = boardWidth - 1; column >= 0; column--)
        {
            rowOffset--;

            if (_cellsArray[column, rowOffset] == (int)playerIcon)
            {
                currentPlayerScore++;
                enemyPlayerScrore = 0;

                if (currentPlayerScore == chosenGameModeSO.modeRequiredComboToWin)
                {
                    winningPlayer = playerIcon;

                    return true;
                }
            }
            else
            {
                if (_cellsArray[column, rowOffset] != EMPTY_CELL_INDEX)
                {
                    currentPlayerScore = 0;
                    enemyPlayerScrore++;

                    if (enemyPlayerScrore == chosenGameModeSO.modeRequiredComboToWin)
                    {
                        winningPlayer = ReturnOtherPlayerIcon(playerIcon);

                        return true;
                    }
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
        // used by the Hint system to pass a hint cell.
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
    }

    public GameModeSO ReturnCurrentGameModeSO()
    {
        return chosenGameModeSO;
    }

    public bool ReturnIsHumanControlling()
    {
        return currentPlayer.publicPlyerData.playerType == PlayerTypes.Human;
    }

    public int[,] ReturnBoardCellsArray()
    {
        int boardWidth = (int)chosenGameModeSO.modeBoardWidthAndHeight.x;
        int boardHeight = (int)chosenGameModeSO.modeBoardWidthAndHeight.y;

        int[,] cellsArrayInt = new int[boardWidth, boardHeight];
        
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                cellsArrayInt[x, y] = cellsArray[x, y].ReturnMarkedIconIndex();
            }
        }

        return cellsArrayInt;
    }

    #endregion

    #region Private Actions
    private void SpawnGridCells()
    {
        int boardWidth = (int)chosenGameModeSO.modeBoardWidthAndHeight.x;
        int boardHeight = (int)chosenGameModeSO.modeBoardWidthAndHeight.y;

        cellsArray = new Cell[boardWidth, boardHeight];

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Cell cell = Instantiate(cellPrefab, cellsParent);
                cell.InitCell(x, y, EMPTY_CELL_INDEX);
                cellsArray[x, y] = cell;
                allEmptyGameCells.Add(cell);

                gameController.ConnectCellToEvents(cell);
            }
        }

        //the starting player is the player with the X type - it's randomised in the factory.
        currentPlayer = players.Where(x => x.publicPlyerData.playerIcon == PlayerIcons.X).FirstOrDefault();
        currentIndexPlayerArray = System.Array.IndexOf(players, currentPlayer);
    }
    #endregion

    #region Mini Max AI Algo
    public Cell CallMiniMaxAlgo()
    {
        // The mini max algorithm allows the AI to check all board possibilities by simulating gameplay, at a given "depth".
        // depth = number of moves the AI is allowed to predict.
        // The higher the depth, the harder the AI becomes.
        // This algorithm is quite expensive - so I have also used the "alpha beta pruning" search algorithm to optemize it.
        // alpha beta pruning basically means that the mini max will stop searching for options once it thinks it has found the best one.


        //this will return a cell to the AI
        int bestScore = int.MinValue;
        Cell chosenCell = null;

        int currentPlayerID = (int)currentPlayer.publicPlyerData.playerIcon;
        int[,] boardCells = ReturnBoardCellsArray(); //get current board as a matrix of ints.

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (boardCells[x, y] == EMPTY_CELL_INDEX)
                {
                    boardCells[x,y] = currentPlayerID;
                    int score = minimax(boardCells, chosenDepthForAlgo, false, ReturnOtherPlayer(currentPlayer.publicPlyerData.playerIcon), int.MinValue, int.MaxValue);
                    boardCells[x, y] = EMPTY_CELL_INDEX;


                    if (score > bestScore)
                    {
                        bestScore = score;
                        chosenCell = cellsArray[x,y];
                    }
                }
            }
        }

        return chosenCell;

    }

    private int minimax(int[,] cellsArray, int depth, bool isMaximizing, PlayerBase player, int alpha, int beta)
    {
        int playerID = (int)player.publicPlyerData.playerIcon;

        if (ReturnGeneralEndConditionMet(out EndConditions endCondition, cellsArray, player, out PlayerIcons winningPlayer) || depth == 0)
        {
            return ReturnAlgoScore(endCondition, winningPlayer, depth);
        }

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (cellsArray[x,y] == EMPTY_CELL_INDEX)
                    {
                        cellsArray[x, y] = playerID;
                        int score = minimax(cellsArray, depth - 1, false, ReturnOtherPlayer(currentPlayer.publicPlyerData.playerIcon), alpha, beta);
                        cellsArray[x, y] = EMPTY_CELL_INDEX;

                        bestScore = Mathf.Max(score, bestScore);

                        alpha = Mathf.Max(alpha, score);
                        if (beta <= alpha)
                        {
                            break; // Beta cutoff
                        }

                    }

                }
            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (cellsArray[x, y] == EMPTY_CELL_INDEX)
                    {
                        cellsArray[x, y] = playerID;
                        int score = minimax(cellsArray, depth - 1, true, ReturnOtherPlayer(player.publicPlyerData.playerIcon), alpha, beta);
                        cellsArray[x, y] = EMPTY_CELL_INDEX;

                        bestScore = Mathf.Min(score, bestScore);

                        beta = Mathf.Min(beta, score);
                        if (beta <= alpha)
                        {
                            break; // Alpha cutoff
                        }
                    }
                }
            }

            return bestScore;
        }
    }

    private int ReturnAlgoScore(EndConditions condition, PlayerIcons winningPlayer, int depth)
    {
        switch (condition)
        {
            case EndConditions.End:
                if (winningPlayer == currentPlayer.publicPlyerData.playerIcon) return 10 + depth;
                else
                {
                    return -10 - depth;
                }
            case EndConditions.Draw:
                return 0;
            default:
                break;
        }

        return 0; // return 0 of there was no end codition to make all cells equal in their value.
    }

    private PlayerBase ReturnOtherPlayer(PlayerIcons currentPlayerIcon)
    {
        //return the next player in the players array, given the current player.

        PlayerBase foundPlayer = players.Where(x => x.publicPlyerData.playerIcon == currentPlayerIcon).FirstOrDefault();
        if(foundPlayer == null)
        {
            Debug.LogError("Error finding player in array of players.");
            return null;
        }

        int playerIndexInArray = System.Array.IndexOf(players, foundPlayer);
        playerIndexInArray++;

        if(playerIndexInArray >= players.Length)
        {
            playerIndexInArray = 0;
        }

        return players[playerIndexInArray];
    }
    private PlayerIcons ReturnOtherPlayerIcon(PlayerIcons currentPlayerIcon)
    {
        int currentPlayerID = (int)currentPlayerIcon;
        currentPlayerID++;

        if(currentPlayerID >= System.Enum.GetValues(typeof(PlayerIcons)).Length)
        {
            currentPlayerID = 0;
        }

        return (PlayerIcons)currentPlayerID;
    }
    #endregion
}
