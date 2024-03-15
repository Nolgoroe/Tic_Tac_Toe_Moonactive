using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditorGameTests
{
    /// <summary>
    /// AAA - ARRANGE, ACT, ASSERT
    /// </summary>

    int currentPlayerIndex = 1;

    #region player win
    [Test, Category("Winning")]
    public void WinRows()
    {
        GameModel model = new GameModel();

        int[,] board = new int[,]
        {
            {0,0,0},
            {1,1,1},
            {0,0,0}
        };


        Assert.AreEqual(true, returnWinRows(board));
    }

    [Test, Category("Winning")]
    public void WinColumns()
    {
        int[,] board = new int[,]
        {
            {1,0,0},
            {1,0,0},
            {1,0,0}
        };


        Assert.AreEqual(true, ReturnWinColumns(board));
    }

    [Test, Category("Winning")]
    public void WinDiagonalLeftBotRightUp()
    {
        int[,] board = new int[,]
        {
            {1,0,1},
            {1,1,0},
            {1,0,0}
        };


        Assert.AreEqual(true, ReturnWinDiagonalLeftBotRightUp(board));
    }

    [Test, Category("Winning")]
    public void WinDiagonalRightBotLeftUp()
    {

        int[,] board = new int[,]
        {
            {1,0,0},
            {0,1,0},
            {0,0,1}
        };


        Assert.AreEqual(true, ReturnWinDiagonalRightBotLeftUp(board));
    }
    #endregion

    #region player lose
    [Test, Category("Losing")]
    public void LoseRows()
    {
        GameModel model = new GameModel();

        int[,] board = new int[,]
        {
            {2,2,2},
            {0,1,1},
            {1,0,0}
        };

        Assert.AreEqual(false, returnWinRows(board));
    }

    [Test, Category("Losing")]
    public void LoseColumns()
    {
        int[,] board = new int[,]
        {
            {1,0,2},
            {1,1,2},
            {0,0,2}
        };

        Assert.AreEqual(false, ReturnWinColumns(board));
    }

    [Test, Category("Losing")]
    public void LoseDiagonalLeftBotRightUp()
    {
        int[,] board = new int[,]
        {
            {1,1,2},
            {0,2,1},
            {2,0,0}
        };

        Assert.AreEqual(false, ReturnWinDiagonalLeftBotRightUp(board));
    }

    [Test, Category("Losing")]
    public void LoseDiagonalRightBotLeftUp()
    {

        int[,] board = new int[,]
        {
            {2,0,1},
            {1,2,0},
            {0,1,2}
        };

        Assert.AreEqual(false, ReturnWinDiagonalRightBotLeftUp(board));
    }
    #endregion

    #region Draw
    [Test, Category("Draw")]
    public void Draw()
    {
        /// 0 in this dummy array means "Empty"
        int[,] board = new int[,]
        {
            {2,1,2},
            {1,1,2},
            {2,2,1}
        };


        Assert.AreEqual(true, CheckDraw(board));
    }

    bool CheckDraw(int[,] board)
    {
        ////check all other options of end - then check if board is full - if it is, draw.

        if (returnWinRows(board) || 
            ReturnWinColumns(board) || 
            ReturnWinDiagonalLeftBotRightUp(board) ||
            ReturnWinDiagonalRightBotLeftUp(board))
        {
            return false;
        }

        /// 0 in this dummy array means "Empty"
        foreach (int playerTestIndex in board)
        {
            if (playerTestIndex == 0)
                return false;
        }

        return true;
    }
    #endregion

    #region Undo
    [Test, Category("Undo")]
    public void Undo()
    {
        int moveBackOnUndo = 2;
        int expectedListCount = 4;

        List<Cell> localDummyList = new List<Cell>();
        for (int i = 0; i < 6; i++)
        {
            localDummyList.Add(new Cell());
        }

        int localListCount = localDummyList.Count;
        for (int i = localListCount - 1; i >= localListCount - moveBackOnUndo; i--)
        {
            localDummyList.RemoveAt(i);
            //this is where I will also reset cell data in the cell class.
        }


        Assert.AreEqual(expectedListCount, localDummyList.Count);
    }
    #endregion

    #region End Functions
    bool returnWinRows(int[,] board)
    {
        for (int row = 0; row < 3; row++)
        {
            int currentPlayerScore = 0;
            int enemyPlayerScrore = 0;

            for (int column = 0; column < 3; column++)
            {
                if (board[row, column] == currentPlayerIndex)
                {
                    currentPlayerScore++;
                    enemyPlayerScrore = 0;

                    if (currentPlayerScore == 3)
                    {
                        return true;
                    }
                }
                else
                {
                    if (board[row, column] != 0)
                    {
                        currentPlayerScore = 0;
                        enemyPlayerScrore++;

                        if (enemyPlayerScrore == 3)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return false;
    }
    bool ReturnWinColumns(int[,] board)
    {
        for (int column = 0; column < 3; column++)
        {
            int currentPlayerScore = 0;
            int enemyPlayerScrore = 0;

            for (int row = 0; row < 3; row++)
            {
                if (board[row, column] == currentPlayerIndex)
                {
                    currentPlayerScore++;
                    enemyPlayerScrore = 0;

                    if (currentPlayerScore == 3)
                    {
                        return true;
                    }
                }
                else
                {
                    if (board[row, column] != 0)
                    {
                        currentPlayerScore = 0;
                        enemyPlayerScrore++;

                        if (enemyPlayerScrore == 3)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return false;
    }
    bool ReturnWinDiagonalLeftBotRightUp(int[,] board)
    {
        int currentPlayerScore = 0;
        int enemyPlayerScrore = 0;

        int rowOffset = 3;

        for (int column = 0; column < 3; column++)
        {
            rowOffset--;

            if (board[rowOffset, column] == currentPlayerIndex)
            {
                currentPlayerScore++;
                enemyPlayerScrore = 0;

                if (currentPlayerScore == 3)
                {
                    return true;
                }
            }
            else
            {
                if (board[rowOffset, column] != 0)
                {
                    currentPlayerScore = 0;
                    enemyPlayerScrore++;

                    if (enemyPlayerScrore == 3)
                    {
                        return false;
                    }
                }
            }
        }

        return false;
    }
    bool ReturnWinDiagonalRightBotLeftUp(int[,] board)
    {
        int currentPlayerScore = 0;
        int enemyPlayerScrore = 0;

        int columnOffset = 3;
        int rowOffset = 3;

        for (int column = columnOffset - 1; column >= 0; column--)
        {
            rowOffset--;

            if (board[rowOffset, column] == currentPlayerIndex)
            {
                currentPlayerScore++;
                enemyPlayerScrore = 0;

                if (currentPlayerScore == 3)
                {
                    return true;
                }
            }
            else
            {
                if (board[rowOffset, column] != 0)
                {
                    currentPlayerScore = 0;
                    enemyPlayerScrore++;

                    if (enemyPlayerScrore == 3)
                    {
                        return false;
                    }
                }
            }
        }

        return false;
    }
    #endregion
}
