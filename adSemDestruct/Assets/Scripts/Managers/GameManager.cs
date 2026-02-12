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
    public static GameManager instance;
    [SerializeField] GameState curState;
    void Start()
    {
        if(instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
