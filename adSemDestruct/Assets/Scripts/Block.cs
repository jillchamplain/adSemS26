using UnityEngine;

public class Block : MonoBehaviour, IGrabbable
{
   [SerializeField] LayerMask interactMask;

    public delegate void BlockPlaced(Block block, GridPiece gridPiece);
    public static event BlockPlaced blockPlaced;
   public void Grabbed(Vector2 pos)
    {
        transform.position = pos;
    }

    public void Released()
    {
        //raycast from center of block and assign to grid square 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, 0, interactMask);
        if(hit)
        {
            if (hit.transform.gameObject.GetComponent<GridPiece>())
            {
                Debug.Log("hit gridPiece");
                blockPlaced?.Invoke(this, hit.transform.gameObject.GetComponent<GridPiece>());
            }
        }
    }
}

