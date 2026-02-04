using UnityEngine;

public class GridPiece : MonoBehaviour
{
    [SerializeField] public GridBased gridBased;
    [Header("References")]
    [SerializeField] BoxCollider2D boxCollider;
    public BoxCollider2D getCollider() {  return boxCollider; }

}
