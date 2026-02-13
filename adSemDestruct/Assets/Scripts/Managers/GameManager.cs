using UnityEngine;

public enum GameState
{
    MENU,
    PAUSED,
    PLAY,
    GAME_OVER,
    GAME_WIN,
    NUM_STATES
}
public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance;
    [SerializeField] GameState curState = GameState.PLAY;
    void setGameState(GameState state) 
    {
        curState = state; 
        switch(curState)
        {
            case GameState.PAUSED:
                break;
            case GameState.MENU:
                break;
            case GameState.GAME_OVER:
                break;
            case GameState.GAME_WIN:
                break;
        }
    }

    // EVENTS

    public delegate void GameStateChangeTo(GameState state);
    public static event GameStateChangeTo gameStateChangeTo;



    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        DestructionGoalManager.destructionGoalsDestroyed += GameWin;
        DestructionGoalManager.destructionGoalsNotDestroyed += GameOver;
    }

    private void OnDisable()
    {
        DestructionGoalManager.destructionGoalsDestroyed -= GameWin;
        DestructionGoalManager.destructionGoalsNotDestroyed -= GameOver;
    }

    void GameWin()
    {
        Debug.Log("Win");
        setGameState(GameState.GAME_WIN);
        gameStateChangeTo?.Invoke(GameState.GAME_WIN);
    }

    void GameOver()
    {
        Debug.Log("Lose");
        setGameState(GameState.GAME_OVER);
        gameStateChangeTo?.Invoke(GameState.GAME_OVER);
    }
}
