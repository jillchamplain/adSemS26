using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [HideInInspector] ScoreManager instance;
    [SerializeField] int score;
    [Header("References")]
    [SerializeField] TextMeshProUGUI scoreTF;

    #region EVENTS

    private void OnEnable()
    {
        LevelMaterial.levelMaterialDestroyed += AddScore;
        LevelGoal.levelGoalDestroyed += AddScore;
    }
    #endregion

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    void AddScore(LevelMaterial levelMaterial)
    {
        score += levelMaterial.scoreValue;
        UpdateUI();
    }

    void AddScore(LevelGoal levelGoal)
    {
        score += levelGoal.scoreValue;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreTF.text = score.ToString();
    }
}
