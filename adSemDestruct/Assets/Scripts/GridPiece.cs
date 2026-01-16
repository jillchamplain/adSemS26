using UnityEngine;

public class GridPiece : MonoBehaviour
{
    [SerializeField] int row;
    public int getRow() {  return row; }
    public void setRow(int newRow) {  row = newRow; }
    [SerializeField] int col;
    public int getCol() { return col; }
    public void setCol(int newCol) { col = newCol; }

    [Header("References")]
    [SerializeField] BoxCollider2D boxCollider;
    public BoxCollider2D getCollider() {  return boxCollider; }

}
