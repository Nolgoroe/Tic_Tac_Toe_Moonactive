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

    public int currentPlayerIndex = 1;

    #region player win
    [Test, Category("Winning")]
    public void WinRows()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {-1, -1, -1},
            {1, 1, 1},
            {-1, -1, -1}
        };

        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.

        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.End, endCondition);
        Assert.AreEqual(winningPlayerIcon, player.publicPlyerData.playerIcon);
    }


    [Test, Category("Winning")]
    public void WinColumns()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {1,-1,-1},
            {1,-1,-1},
            {1,-1,-1}
        };
        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.


        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.End, endCondition);
        Assert.AreEqual(winningPlayerIcon, player.publicPlyerData.playerIcon);
    }

    [Test, Category("Winning")]
    public void WinDiagonalLeftBotRightUp()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {-1,-1,1},
            {-1,1,-1},
            {1,-1,-1}
        };
        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.

        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.End, endCondition);
        Assert.AreEqual(winningPlayerIcon, player.publicPlyerData.playerIcon);
    }

    [Test, Category("Winning")]
    public void WinDiagonalRightBotLeftUp()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {1,-1,-1},
            {-1,1,-1},
            {-1,-1,1}
        };
        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.


        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.End, endCondition);
        Assert.AreEqual(winningPlayerIcon, player.publicPlyerData.playerIcon);
    }
    #endregion

    #region player lose
    [Test, Category("Losing")]
    public void LoseRows()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {0,0,0},
            {-1,1,1},
            {1,-1,-1}
        };

        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.

        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.End, endCondition);
        Assert.AreNotEqual(winningPlayerIcon, player.publicPlyerData.playerIcon);
    }

    [Test, Category("Losing")]
    public void LoseColumns()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {1,-1,0},
            {1,1,0},
            {-1,-1,0}
        };

        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.

        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.End, endCondition);
        Assert.AreNotEqual(winningPlayerIcon, player.publicPlyerData.playerIcon);
    }

    [Test, Category("Losing")]
    public void LoseDiagonalLeftBotRightUp()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {1,1,0},
            {-1,0,1},
            {0,-1,-1}
        };

        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.

        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.End, endCondition);
        Assert.AreNotEqual(winningPlayerIcon, player.publicPlyerData.playerIcon);
    }

    [Test, Category("Losing")]
    public void LoseDiagonalRightBotLeftUp()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {0,-1,1},
            {1,0,-1},
            {-1,1,0}
        };

        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.

        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.End, endCondition);
        Assert.AreNotEqual(winningPlayerIcon, player.publicPlyerData.playerIcon);
    }
    #endregion

    #region Draw
    [Test, Category("Draw")]
    public void Draw()
    {
        GameModel model = GameObject.FindObjectOfType<GameModel>();
        model.UnitTestSetGameMode();
        PlayerBase player = new HumanPlayer("test Player", null, PlayerTypes.Human, PlayerIcons.O);

        int[,] board = new int[,]
        {
            {0,1,0},
            {1,1,0},
            {0,0,1}
        };

        board = FlipArray(board); //we flip the array to make it the same as our game board - where 0,0 is the top left.

        model.ReturnGeneralEndConditionMet(out EndConditions endCondition, board, player, out PlayerIcons winningPlayerIcon);

        Assert.AreEqual(EndConditions.Draw, endCondition);
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

    private int[,] FlipArray(int[,] board)
    {
        int rowCount = board.GetLength(0); // Number of rows
        int colCount = board.GetLength(1); // Number of columns

        // Creating a new array with flipped dimensions
        int[,] flippedBoard = new int[colCount, rowCount];

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                flippedBoard[j, i] = board[i, j]; // Assigning flipped values
            }
        }

        return flippedBoard;
    }

}
