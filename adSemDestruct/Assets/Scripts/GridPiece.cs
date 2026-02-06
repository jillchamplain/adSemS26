using UnityEngine;

public class GridPiece : GridBased
{
    [SerializeField] MatchItem matchItemRef;
    public MatchItem getMatchItem() {  return matchItemRef; }
    public void setMatchItem(MatchItem newRef) 
    {
        if(newRef == null)
        {
            matchItemRef = null;
            return;
        }

        Debug.Log(this + "assigning");

        newRef.setPrevRow(newRef.row);
        newRef.setPrevCol(newRef.col);

        newRef.row = this.row;
        newRef.col = this.col;
        matchItemRef = newRef; 
    }

    [Header("References")]
    [SerializeField] BoxCollider2D boxCollider;
    public BoxCollider2D getCollider() {  return boxCollider; }

}
