using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
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


    public delegate void MatchItemPlaced(MatchItem item, GridPiece gridPiece);
    public static event MatchItemPlaced matchItemPlaced;

    public delegate void MatchItemDestroyed();
    public static event MatchItemDestroyed matchItemDestroyed;

    private void OnEnable()
    {
        MatchGrid.match += OnMatch;
    }

    private void OnDisable()
    {
        MatchGrid.match -= OnMatch;
    }
    private void Start()
    {
        prevRow = row;
        prevCol = col;
    }

    void OnMatch(List<GridPiece> matchPieces, Vector3 origin, BlockShape shape, MatchItemType type)
    {
        foreach (GridPiece gp in matchPieces)
        {
            if(gp.getMatchItem() == this)
            {
                gp.setMatchItem(null);
                matchItemDestroyed?.Invoke();
                Destroy(gameObject); //need to unassign from grid
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
            if (hit.transform.gameObject.GetComponent<GridPiece>())
            {
                // Debug.Log("hit gridPiece");
                matchItemPlaced?.Invoke(this, hit.transform.gameObject.GetComponent<GridPiece>());
            }
        }
    }
    #endregion
}

