using UnityEngine;
public enum MatchItemType
{
    A,
    B,
    C,
    NUM_TYPES
}

public class MatchItem : MonoBehaviour, IGrabbable
{
    [SerializeField] public GridBased gridBased;
    [SerializeField] MatchItemType type;
    [SerializeField] LayerMask interactMask;

    public delegate void MatchItemPlaced(MatchItem item, GridPiece gridPiece);
    public static event MatchItemPlaced matchItemPlaced;

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
}

