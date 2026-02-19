using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class LevelGoalManager : MonoBehaviour, ISubManager
{
    [HideInInspector] public static LevelGoalManager instance;
    [Header("Data")]
    [SerializeField] List<GameObject> levelGoals = new List<GameObject>();
    [Header("References")]
    [SerializeField] GameObject levelGoalPF;

    #region EVENTS
    public delegate void LevelGoalsDestroyed();
    public static event LevelGoalsDestroyed levelGoalsDestroyed;

    public delegate void LevelGoalsNotDestroyed();
    public static event LevelGoalsNotDestroyed levelGoalsNotDestroyed;

    private void OnEnable()
    {
        LevelGoal.levelGoalDestroyed += RemoveLevelGoal;
        TurnManager.reachedMaxTurns += LoseLevelGoalsCheck;
    }

    private void OnDisable()
    {
        LevelGoal.levelGoalDestroyed -= RemoveLevelGoal;
        TurnManager.reachedMaxTurns -= LoseLevelGoalsCheck;
    }
    #endregion

    private void Start()
    {
        //SpawnDestructionGoal(new Vector3(1, 1, 0));
        
    }

    void RemoveLevelGoal(LevelGoal theGoal)
    {
        levelGoals.Remove(theGoal.gameObject);
        WinLevelGoalsCheck();
        
    }
    void LoseLevelGoalsCheck()
    {
        if (levelGoals.Count > 0)
            levelGoalsNotDestroyed?.Invoke();
        else
            WinLevelGoalsCheck();
    }

    void WinLevelGoalsCheck()
    {
        if(levelGoals.Count <= 0)
        {
            levelGoalsDestroyed?.Invoke();
        }
    }

    void SpawnDestructionGoal(Vector3 pos) //Level scriptable object?
    {
        GameObject newGoal = Instantiate(levelGoalPF, pos, Quaternion.identity);
        levelGoals.Add(newGoal);

        newGoal.transform.parent = this.gameObject.transform;
    }

    #region ISubManager
    public void HandleGameState(GameState state)
    {

    }
    #endregion
}
