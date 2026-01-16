using UnityEngine;

public class Block : MonoBehaviour, IGrabbable
{
   public void Grabbed(Vector2 pos)
    {
        transform.position = pos;
    }

    public void Released()
    {
        //raycast from center of block and assign to grid square 
    }
}
