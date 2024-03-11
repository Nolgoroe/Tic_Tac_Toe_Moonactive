using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    const int EMPTY_CELL_INDEX = -1;

    [SerializeField] Image cellImage;
    [SerializeField] bool isMarked;
    [SerializeField] int MarkedIconIndex = EMPTY_CELL_INDEX; // this is set by the player enum - later used for checks. starts at -1 to indicate empty.
    [SerializeField] Vector2 cellCoordinates = Vector2.zero; // this is set by the player enum - later used for checks. starts at -1 to indicate empty.

    public Action<Cell> OnClickOnCell; //temp public? do we need this event?

    private void SetCellSprite(Sprite icon)
    {
        cellImage.sprite = icon;
    }
    private void SetIsMarked(bool _isMarked)
    {
        isMarked = _isMarked;
    }
    private void SetMarkingPlayerIndex(int _MarkedIconIndex)
    {
        MarkedIconIndex = _MarkedIconIndex;
    }

    public void ActivateOnClickOnCellAction()
    {
        OnClickOnCell?.Invoke(this);
    }


    public void InitCell(int x, int y)
    {
        cellCoordinates = new Vector2(x, y);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMarked) return; //display a ui message here that the cell is not empty in the end. //temp

        Debug.Log("Detect Click");

        ActivateOnClickOnCellAction();
    }

    public void MarkCell(PlayerBase currentPlayer)
    {
        //is it ok for this function.. and other functions to be public because they are used as actions?
        PlayerData currentPlayerData = currentPlayer.publicPlyerData;

        SetCellSprite(currentPlayerData.playerIconSprite);
        SetIsMarked(true);
        SetMarkingPlayerIndex((int)currentPlayerData.playerIconIndex);
    }

    //public void UnMarkCell()
    //{
    //    SetCellSprite(null); //destory the cell in the slot??
    //    SetIsMarked(false);
    //    SetMarkingPlayerIndex(EMPTY_CELL_INDEX); // -1 is default for empty.
    //}

    public bool ReturnIsMarked()
    {
        return isMarked;
    }
    public int ReturnMarkedIconIndex()
    {
        return MarkedIconIndex;
    }

    private void OnDisable()
    {
        OnClickOnCell = null;
    }
}
