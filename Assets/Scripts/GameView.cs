using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] PlayerBase currentPlayer;

    public void InitGameView()
    {
        //nothing for now - maybe we don't need this function.
    }

    public void UpdateCurrentPlayerRef(PlayerBase _currentPlayer)
    {
        currentPlayer = _currentPlayer;
        //also update name of player in UI
        // also maybe animate something...
    }

    public void UpdateCellView(Cell selectedCell)
    {
        selectedCell.MarkCell(currentPlayer);
    }
}
