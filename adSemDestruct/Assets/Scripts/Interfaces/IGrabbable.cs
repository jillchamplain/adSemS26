using UnityEngine;

public interface IGrabbable 
{
    public void Grabbed(Vector2 pos);

    public void Released();
}
