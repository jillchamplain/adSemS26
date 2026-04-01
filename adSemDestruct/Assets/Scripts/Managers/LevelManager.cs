using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public LevelManager instance; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RestartLevel()
    {
        string curLevel = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(curLevel);
    }

    public void GoToLevelIndex(int level)
    {
        SceneManager.LoadScene(level);
    }


    public void GoToNextLevel()
    {
        //Get level build index
        //Increase and go to next build index
        int curLevelIndex = SceneManager.GetActiveScene().buildIndex;
        curLevelIndex++;

        //If last scene go to main menu
        if (curLevelIndex >= SceneManager.sceneCountInBuildSettings)
            curLevelIndex = 0;

        SceneManager.LoadScene(curLevelIndex);
    }
}
