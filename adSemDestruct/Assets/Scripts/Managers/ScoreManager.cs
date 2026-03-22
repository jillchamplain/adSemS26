using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [HideInInspector] public static ScoreManager instance;
    [SerializeField] int score;
    [Header("References")]
    [SerializeField] TextMeshProUGUI scoreTF;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    public void AddScore(int scoreValue)
    {
        score += scoreValue;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreTF.text = score.ToString();
    }
}
