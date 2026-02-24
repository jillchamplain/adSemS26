using UnityEngine;

public abstract class Destructor : MonoBehaviour 
{
    [Header("Data")]
    [SerializeField] Rigidbody2D rb;
    public void setRigidBody(Rigidbody2D rb){ this.rb = rb; }
    [SerializeField] float forceDamageMult = 0;
    public void setForceDamageMult(float newMult) { forceDamageMult = newMult; } //Set these based on match type or level material type
    [SerializeField] float forceDamageMin;
    public void setForceDamageMin(float newMin) { forceDamageMin = newMin; }
    [SerializeField] float weight = 1;
    public void setWeight(float newWeight) {  weight = newWeight; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
		rb.mass = weight;
	}
    public float CalcForceDamage()
    {
        
        float forceDamage = forceDamageMult * Mathf.Abs(this.gameObject.GetComponent<Rigidbody2D>().linearVelocityY);
        return Mathf.Max(forceDamageMin,forceDamage);
        
    }
}
