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
        
        if (matchItemRef != null)
        {
            int holdR = matchItemRef.row;
            int holdC = matchItemRef.col;

            matchItemRef.row = matchItemRef.getPrevRow();
            matchItemRef.setPrevRow(holdR);

            matchItemRef.col = matchItemRef.getPrevCol();
            matchItemRef.setPrevCol(holdC);
        }

        newRef.row = this.row;
        newRef.col = this.col;
        matchItemRef = newRef; 
    }

    [Header("References")]
    [SerializeField] BoxCollider2D boxCollider;
    public BoxCollider2D getCollider() {  return boxCollider; }

}
