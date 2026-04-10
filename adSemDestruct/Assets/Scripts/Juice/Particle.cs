using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] float lifeTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (GetComponentInChildren<ParticleSystem>())
        {
            lifeTime = GetComponentInChildren<ParticleSystem>().main.duration;
            //Debug.Log($"New lifetime is {lifeTime}!");
        }
    }
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Spawn()
    {

    }

    protected void Play()
    {

    }
}
