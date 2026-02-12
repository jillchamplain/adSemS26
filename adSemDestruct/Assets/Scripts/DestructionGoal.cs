using UnityEngine;

public class DestructionGoal : MonoBehaviour
{
    //EVENTS
    public delegate void DestructionGoalDestroyed();
    public static event DestructionGoalDestroyed destructionGoalDestroyed;

    void DestroySelf()
    {
        destructionGoalDestroyed?.Invoke();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroySelf();
    }
}
