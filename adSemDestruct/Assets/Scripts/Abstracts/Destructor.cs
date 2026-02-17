using UnityEngine;

public abstract class Destructor : MonoBehaviour 
{
    [Header("Data")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float forceDamageMult;
    [SerializeField] float weight;

    private void Start()
    {
        rb.mass = weight;
    }
    public int CalcForceDamage()
    {
        int forceDamage = (int)(forceDamageMult * Mathf.Abs(this.gameObject.GetComponent<Rigidbody2D>().linearVelocityY));
        return forceDamage;
    }
}
