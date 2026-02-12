using UnityEngine;

public enum GameState
{
    MENU,
    PAUSED,
    GAME_OVER,
    GAME_WIN,
    NUM_STATES
}
public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance;
    [SerializeField] GameState curState;
    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        DestructionGoalManager.destructionGoalsDestroyed += GameWon;
    }

    private void OnDisable()
    {
        DestructionGoalManager.destructionGoalsDestroyed -= GameWon;
    }

    void GameWon()
    {
        curState = GameState.GAME_WIN;
    }

    void GameOver()
    {
        curState = GameState.GAME_OVER;
    }
}
