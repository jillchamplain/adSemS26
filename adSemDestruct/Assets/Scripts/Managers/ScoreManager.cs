using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [HideInInspector] public static ScoreManager instance;
    [SerializeField] int score;
    [Header("References")]
    [SerializeField] TextMeshProUGUI scoreTF;
    [SerializeField] GameObject scoreParticlePF;
    [SerializeField] Vector3 scoreParticleSpawn;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    public void AddScore(int scoreValue)
    {
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(scoreParticleSpawn);
        spawnPos = new Vector3(spawnPos.x, spawnPos.y, 0);
        Debug.Log(spawnPos);
        GameObject scoreParticle = Instantiate(scoreParticlePF, spawnPos, Quaternion.identity);
        scoreParticle.GetComponentInChildren<TextMeshPro>().text = "+" + scoreValue.ToString();
        scoreParticle.transform.DOLocalMoveY(.1f, 5f);
        scoreParticle.GetComponentInChildren<TextMeshPro>().DOFade(0f, 1f);

        score += scoreValue;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreTF.text = score.ToString();
    }
}
