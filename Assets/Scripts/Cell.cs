using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image cellImage;
    [SerializeField] bool isMarked;
    [SerializeField] int MarkedIconIndex = -1; // this is set by the player enum - later used for checks. starts at -1 to indicate empty.

    Action<Cell> OnClickOnCell;

    private void SetCellSprite(Sprite icon)
    {
        cellImage.sprite = icon;
    }
    public void MarkCell()
    {
        OnClickOnCell?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Detect Click");

        MarkCell();
    }

    private void OnDisable()
    {
        OnClickOnCell = null;
    }
}
