using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class DestructionGoalManager : MonoBehaviour
{
    [HideInInspector] public static DestructionGoalManager instance;
    [SerializeField] List<GameObject> destructionGoals = new List<GameObject>();

    //EVENTS
    public delegate void DestructionGoalsDestroyed();
    public static event DestructionGoalsDestroyed destructionGoalsDestroyed;

    public delegate void DestructionGoalsNotDestroyed();
    public static event DestructionGoalsNotDestroyed destructionGoalsNotDestroyed;

    private void OnEnable()
    {
        DestructionGoal.destructionGoalDestroyed += RemoveDestructionGoal;
    }

    private void OnDisable()
    {
        DestructionGoal.destructionGoalDestroyed -= RemoveDestructionGoal;
    }

    void RemoveDestructionGoal(DestructionGoal theGoal)
    {
        destructionGoals.Remove(theGoal.gameObject);

        
    }

    void FinalDestructionGoalsCheck()
    {
        if (destructionGoals.Count > 0)
            destructionGoalsNotDestroyed?.Invoke();
    }

    void DestructionGoalsCheck()
    {
        if(destructionGoals.Count <= 0)
        {
            destructionGoalsDestroyed?.Invoke();
        }
        else
        {

        }
    }

    void SpawnDestructionGoal(GameObject theGoal, Vector3 pos)
    {

    }
}
