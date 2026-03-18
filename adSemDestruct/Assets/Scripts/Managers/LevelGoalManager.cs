using UnityEngine;
using System.Collections.Generic;
public class LevelGoalManager : MonoBehaviour, ISubManager
{
    [HideInInspector] public static LevelGoalManager instance;
    [SerializeField] float timeTillGameOver;
    [SerializeField] List<GameObject> levelGoals;
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
        TurnManager.reachedMaxTurns += LevelGoalsCheck;
        GameManager.tryToGameOver += LevelGoalsCheck;
    }

    private void OnDisable()
    {
        LevelGoal.levelGoalDestroyed -= RemoveLevelGoal;
        TurnManager.reachedMaxTurns -= LevelGoalsCheck;
        GameManager.tryToGameOver -= LevelGoalsCheck;
    }
    #endregion

    private void Start()
    {
        LevelGoal[] goals = FindObjectsByType<LevelGoal>(FindObjectsSortMode.InstanceID);
        foreach (var goal in goals)
        {
            levelGoals.Add(goal.gameObject);
        }
    }

    void RemoveLevelGoal(LevelGoal theGoal)
    {
        levelGoals.Remove(theGoal.gameObject);
        LevelGoalsWinCheck();

    }

    void LevelGoalsWinCheck()
    {
        if (levelGoals.Count <= 0)
            levelGoalsDestroyed?.Invoke();
    }

    void LevelGoalsCheck()
    {
        if (levelGoals.Count > 0)
            levelGoalsNotDestroyed?.Invoke();

        else
            levelGoalsDestroyed?.Invoke();
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
