using TMPro;
using UnityEngine;

public interface IScoreable
{
    public void GiveScore()
    {
        ScoreManager.instance.AddScore(Score);
    }
    [SerializeField] int Score { get; }
    [SerializeField] GameObject ScoreParticlePF { get; }
}
