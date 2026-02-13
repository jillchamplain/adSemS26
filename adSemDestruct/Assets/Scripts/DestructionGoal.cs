using UnityEngine;

public class DestructionGoal : MonoBehaviour
{
    //EVENTS
    public delegate void DestructionGoalDestroyed(DestructionGoal theGoal);
    public static event DestructionGoalDestroyed destructionGoalDestroyed;

    void DestroySelf()
    {
        destructionGoalDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Block>())
            DestroySelf();
    }
}
