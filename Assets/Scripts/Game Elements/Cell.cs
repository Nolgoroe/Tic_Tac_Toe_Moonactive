using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    const int EMPTY_CELL_INDEX = -1; //-1 indicates "empty" cell

    public Action<Cell> OnClickOnCell;
    public Action<Cell> OnRemoveCell;

    [Header("References")]
    [SerializeField] Image cellImage;
    [SerializeField] ParticleSystem hintParticle;

    [Header("Gameplay")]
    [SerializeField] bool isMarked;
    [SerializeField] int MarkedIconIndex = EMPTY_CELL_INDEX; // this is set by the player enum - later used for checks. starts at -1 to indicate empty.
    [SerializeField] Vector2Int cellCoordinates = new Vector2Int(-1,-1);

    [Header("Animation")]
    [SerializeField] float scaleUpSpeed;

    #region Initialization
    public void InitCell(int x, int y)
    {
        cellCoordinates = new Vector2Int(x, y);
    }

    //default constructor
    public Cell()
    {

    }

    //copy constructor
    public Cell(Cell cellToCopy)
    {
        isMarked = cellToCopy.isMarked;
        MarkedIconIndex = cellToCopy.MarkedIconIndex;
        cellCoordinates = cellToCopy.cellCoordinates;
    }
    #endregion

    #region Marking and Unmarking
    public void MarkCell(PlayerBase currentPlayer)
    {
        PlayerData currentPlayerData = currentPlayer.publicPlyerData;

        SetCellSprite(currentPlayerData.playerIconSprite);
        SetIsMarked(true);
        SetMarkingPlayerIndex((int)currentPlayerData.playerIcon);
    }
    public void AnimateMark()
    {
        cellImage.transform.localScale = Vector3.zero;

        LeanTween.scale(cellImage.gameObject, Vector3.one, scaleUpSpeed).setEaseOutBack();
        SoundManager.Instance.PlaySoundOneShot(Sounds.MarkCell);
    }

    private void SetCellSprite(Sprite icon)
    {
        cellImage.sprite = icon;
        cellImage.enabled = icon == null ? false : true;
    }
    private void SetIsMarked(bool _isMarked)
    {
        isMarked = _isMarked;
    }
    private void SetMarkingPlayerIndex(int _MarkedIconIndex)
    {
        MarkedIconIndex = _MarkedIconIndex;
    }

    public void UnMarkCell()
    {
        SetCellSprite(null);
        SetIsMarked(false);
        SetMarkingPlayerIndex(EMPTY_CELL_INDEX); // -1 is default for empty.
    }

    #endregion

    #region Input Detection
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMarked || !GameController.Instance.ReturnCurrentPlayerIsHuman()) return;

        ActivateOnClickOnCellAction();
    }

    public void ActivateOnClickOnCellAction()
    {
        //called directly from the AI player aswell to simulate click

        if (GameController.isGameOver) return;

        OnClickOnCell?.Invoke(this);
    }


    #endregion

    #region Public Return Data
    public bool ReturnIsMarked()
    {
        return isMarked;
    }
    public int ReturnMarkedIconIndex()
    {
        return MarkedIconIndex;
    }
    public Vector2Int ReturnCellCoordinatesInBoard()
    {
        return cellCoordinates;
    }
    public int ReturnEmptyCellIndex()
    {
        return EMPTY_CELL_INDEX;
    }
    #endregion

    #region Public Actions
    public void SetAsHint()
    {
        //spawn effect here that dies after X seconds.
        hintParticle.gameObject.SetActive(true);
        hintParticle.Play();
    }
    #endregion

    private void OnDisable()
    {
        OnClickOnCell = null;
        OnRemoveCell = null;
    }
}
