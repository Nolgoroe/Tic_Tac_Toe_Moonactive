using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoSystem : MonoBehaviour
{
    [SerializeField] List<Cell> cellsMarked;
    [SerializeField] int moveBackOnUndo = 2;


    public void AddCellToMarkedList(Cell cell)
    {
        cellsMarked.Add(cell);
    }

    public void RemoveCellsFromMarkedList()
    {
        //this is called from a button in the scene

        if(cellsMarked.Count < moveBackOnUndo)
        {
            Debug.Log("Can't undo");
            return;
        }

        int originalListCount = cellsMarked.Count;
        for (int i = originalListCount - 1; i >= originalListCount - moveBackOnUndo; i--)
        {
            cellsMarked[i].UnMarkCell();
            cellsMarked.RemoveAt(i);
        }
    }
}
