using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
public enum MatchItemType
{
    A,
    B,
    C,
    D,
    E,
    F,
    NUM_TYPES
}

public class MatchItem : GridBased, IGrabbable
{
    [SerializeField] MatchItemType type;
    public MatchItemType getType() {  return type; }
    public void setType(MatchItemType type) { this.type = type; }

    [SerializeField] int prevRow;
    public int getPrevRow() { return prevRow; }
    public void setPrevRow(int prevRow) { this.prevRow = prevRow; }
    [SerializeField] int prevCol;

    public int getPrevCol() { return prevCol; }
    public void setPrevCol(int prevCol) { this.prevCol = prevCol; }

    [SerializeField] LayerMask interactMask;

    #region EVENTS
    public delegate void MatchItemPlaced(MatchItem item, GridPiece gridPiece);
    public static event MatchItemPlaced matchItemPlaced;

    public delegate void MatchItemDestroyed(int x, int y); //Need standard of events. Called from only manager or object
    public static event MatchItemDestroyed matchItemDestroyed;

    private void OnEnable()
    {
        MatchGrid.match += OnMatch;
    }

    private void OnDisable()
    {
        MatchGrid.match -= OnMatch;
    }
#endregion

    private void Start()
    {
        prevRow = row;
        prevCol = col;
    }

    public void DestroySelfCall()
    {
        transform.DOPunchScale(new Vector3(2, 2, 0), 0.5f);
        //Debug.Log("Destroy Anim");
        //Debug.Log("Done animating");
        matchItemDestroyed?.Invoke(this.row , this.col);
        Destroy(this.gameObject);
    }


    void OnMatch(List<GridPiece> matchPieces, Vector3 origin, BlockShape shape, MatchItemType type) 
    {
        foreach (GridPiece gp in matchPieces)
        {
            if(gp.getMatchItem() == this)
            {
                DestroySelfCall();
            }
        }
    }

    #region IGrabbable
    public void Grabbed(Vector2 pos)
    {
        transform.position = pos;
    }

    public void Released()
    {
        //raycast from center of block and assign to grid square 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, 0, interactMask);
        if (hit)
        {
            matchItemPlaced?.Invoke(this, hit.transform.gameObject.GetComponent<GridPiece>());
        }
        else
        {
            matchItemPlaced?.Invoke(this, null);
        }
    }
    #endregion
}

