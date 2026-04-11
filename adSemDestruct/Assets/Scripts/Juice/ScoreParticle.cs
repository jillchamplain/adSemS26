using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using DG.Tweening;
public class ScoreParticle : Particle
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(int score)
    {
        GetComponentInChildren<TextMeshPro>().text = "$" + score.ToString();
        GetComponentInChildren<TextMeshPro>().DOFade(0f, 1.75f);
        transform.DOJump(transform.position, 2f, 1, 2f);
    }
}
