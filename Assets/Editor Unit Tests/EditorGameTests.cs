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

    //check all unit tests with Lior - //Temp??
    //There is no connection to the actual gameplay logic except for the for loops.. is that ok?


    // A Test behaves as an ordinary method

    int currentPlayerIndex = 1;/// 1 is human, 2 is AI

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

        currentPlayerIndex = 1;

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

        currentPlayerIndex = 1;

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

        currentPlayerIndex = 1;

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

        currentPlayerIndex = 1;

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

        currentPlayerIndex = 2;
        Assert.AreEqual(true, returnWinRows(board));
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

        currentPlayerIndex = 2;
        Assert.AreEqual(true, ReturnWinColumns(board));
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

        currentPlayerIndex = 2;
        Assert.AreEqual(true, ReturnWinDiagonalLeftBotRightUp(board));
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

        currentPlayerIndex = 2;
        Assert.AreEqual(true, ReturnWinDiagonalRightBotLeftUp(board));
    }
    #endregion




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

        ////check all other options of winning - then check if board is full - if it is, draw.

        Assert.AreEqual(true, CheckDraw(board));

        currentPlayerIndex = 2;

        Assert.AreEqual(true, CheckDraw(board));
    }

    bool CheckDraw(int[,] board)
    {
        if(returnWinRows(board) || 
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

    [Test, Category("Undo")]
    public void Undo()
    {
        //Make sure to check this with Lior for an upgrade. it feels too... simple to me
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








    bool returnWinRows(int[,] board)
    {
        int currentScore = 0;

        for (int row = 0; row < 3; row++)
        {
            currentScore = 0;

            for (int column = 0; column < 3; column++)
            {
                //this is used to look for a series of the same number.
                // this is how we decide on which number to check - more useful in the draw and lose test cases.

                if (board[row, column] == currentPlayerIndex)
                {
                    currentScore++;

                    if (currentScore == 3)
                    {
                        Debug.Log("Win in row");
                        return true;
                    }
                }
            }
        }

        return false;
    }
    bool ReturnWinColumns(int[,] board)
    {
        int currentScore = 0;

        for (int column = 0; column < 3; column++)
        {
            currentScore = 0;

            for (int row = 0; row < 3; row++)
            {
                if (board[row, column] == currentPlayerIndex)
                {
                    currentScore++;

                    if (currentScore == 3)
                    {
                        Debug.Log("Win in coolumn");
                        return true;
                    }
                }
            }
        }

        return false;
    }
    bool ReturnWinDiagonalLeftBotRightUp(int[,] board)
    {
        int currentScore = 0;

        int rowOffset = 3;

        for (int column = 0; column < 3; column++)
        {
            rowOffset--;

            if (board[rowOffset, column] == currentPlayerIndex)
            {
                currentScore++;
                if (currentScore == 3)
                {
                    Debug.Log("Win in diag left bot right up");
                    return true;
                }
            }

        }

        return false;
    }
    bool ReturnWinDiagonalRightBotLeftUp(int[,] board)
    {
        int currentScore = 0;

        int columnOffset = 3;
        int rowOffset = 3;

        for (int column = columnOffset - 1; column >= 0; column--)
        {
            rowOffset--;

            if (board[rowOffset, column] == currentPlayerIndex)
            {
                currentScore++;
                if (currentScore == 3)
                {
                    Debug.Log("Win in diag right bot left up");
                    return true;
                }
            }

        }

        return false;
    }

}
