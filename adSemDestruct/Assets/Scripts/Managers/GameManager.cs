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
    public void setGameState(GameState state)                                                                                                      
    {
        curState = state;
        Time.timeScale = 1f;
        switch(curState)
        {
            case GameState.PAUSED:
                Time.timeScale = 0f;
                break;
            case GameState.MENU:
                break;
            case GameState.GAME_OVER:
                break;
            case GameState.GAME_WIN:
                break;
        }
        gameStateChangeTo?.Invoke(curState);
    }
    #region EVENTS
    public delegate void GameStateChangeTo(GameState state);
    public static event GameStateChangeTo gameStateChangeTo;
    private void OnEnable()
    {
        LevelGoalManager.levelGoalsDestroyed += GameWin;
        LevelGoalManager.levelGoalsNotDestroyed += GameOver;
    }

    private void OnDisable()
    {
        LevelGoalManager.levelGoalsDestroyed -= GameWin;
        LevelGoalManager.levelGoalsNotDestroyed -= GameOver;
    }
    #endregion

    void Awake()
    {
        if (instance == null)
            instance = this;
        setGameState(GameState.PLAY);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(curState != GameState.PAUSED)
                setGameState(GameState.PAUSED);
            else setGameState(GameState.PLAY);
        }
    }

    public void Play()
    {
        setGameState(GameState.PLAY);
    }

    public void Pause()
    {
        setGameState(GameState.PAUSED);
    }

    void GameWin()
    {
        Debug.Log("Win");
        setGameState(GameState.GAME_WIN);
    }

    void GameOver()
    {
        Debug.Log("Lose");
        setGameState(GameState.GAME_OVER);
    }
}
