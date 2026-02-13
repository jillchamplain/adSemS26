using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class DestructionGoalManager : MonoBehaviour, ISubManager
{
    [HideInInspector] public static DestructionGoalManager instance;
    [Header("Data")]
    [SerializeField] List<GameObject> destructionGoals = new List<GameObject>();
    [Header("References")]
    [SerializeField] GameObject destructionGoalPF;

    //EVENTS



    public delegate void DestructionGoalsDestroyed();
    public static event DestructionGoalsDestroyed destructionGoalsDestroyed;

    public delegate void DestructionGoalsNotDestroyed();
    public static event DestructionGoalsNotDestroyed destructionGoalsNotDestroyed;

    private void OnEnable()
    {
        DestructionGoal.destructionGoalDestroyed += RemoveDestructionGoal;
        TurnManager.reachedMaxTurns += LoseDestructionGoalsCheck;
    }

    private void OnDisable()
    {
        DestructionGoal.destructionGoalDestroyed -= RemoveDestructionGoal;
        TurnManager.reachedMaxTurns -= LoseDestructionGoalsCheck;
    }

    private void Start()
    {
        SpawnDestructionGoal(new Vector3(1, 1, 0));
    }

    void RemoveDestructionGoal(DestructionGoal theGoal)
    {
        destructionGoals.Remove(theGoal.gameObject);
        WinDestructionGoalsCheck();
        
    }
    void LoseDestructionGoalsCheck()
    {
        if (destructionGoals.Count > 0)
            destructionGoalsNotDestroyed?.Invoke();
        else
            WinDestructionGoalsCheck();
    }

    void WinDestructionGoalsCheck()
    {
        if(destructionGoals.Count <= 0)
        {
            destructionGoalsDestroyed?.Invoke();
        }
    }

    void SpawnDestructionGoal(Vector3 pos) //Level scriptable object?
    {
        GameObject newGoal = Instantiate(destructionGoalPF, pos, Quaternion.identity);
        destructionGoals.Add(newGoal);

        newGoal.transform.parent = this.gameObject.transform;
    }

    #region ISubManager
    public void HandleGameState(GameState state)
    {

    }
    #endregion
}
