using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoSystem : MonoBehaviour
{
    //The idea of this class is to, on every time a cell is marked - add it to a list of marked cells.
    //This will then give me the option to, on a click on a button, iterate through an X amount of elements in the list and just call the "Unmark" func on those cells.
    
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
            cellsMarked[i].OnRemoveCell?.Invoke(cellsMarked[i]);

            cellsMarked.RemoveAt(i);
        }
    }
}
