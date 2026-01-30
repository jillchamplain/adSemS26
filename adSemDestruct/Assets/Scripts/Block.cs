using UnityEngine;

public class Block : MonoBehaviour, IGrabbable
{
   [SerializeField] LayerMask interactMask
   public void Grabbed(Vector2 pos)
    {
        transform.position = pos;
    }

    public void Released()
    {
        //raycast from center of block and assign to grid square 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward);
        if(hit)
        {
            Debug.Log(hit.transform.gameObject);
        }
    }
}

