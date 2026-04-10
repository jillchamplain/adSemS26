using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using DG.Tweening;
public class ScoreParticle : Particle
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(int score)
    {
        GetComponentInChildren<TextMeshPro>().text = "$" + score.ToString();
        GetComponentInChildren<TextMeshPro>().DOFade(1f, 1f);
        transform.DOLocalMoveY(.1f, 5f);
    }
}
