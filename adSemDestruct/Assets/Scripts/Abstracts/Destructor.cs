using UnityEngine;

public abstract class Destructor : MonoBehaviour 
{
    [Header("Destructor Data")]
    [SerializeField] protected Rigidbody2D rb;
    public void setRigidBody(Rigidbody2D rb){ this.rb = rb; }
    [SerializeField] float forceDamageMult = 0;
    public void setForceDamageMult(float newMult) { forceDamageMult = newMult; } //Set these based on match type or level material type

    [Tooltip("The minimum damage a destructor can do to an object")]
    [SerializeField] float forceDamageMin;
    public void setForceDamageMin(float newMin) { forceDamageMin = newMin; }
    [Tooltip("The minimum damage a destructor needs to reach to register damage")]
    [SerializeField] float forceDamageThreshold;

    [SerializeField] float weightMultiplier = 1;
    public void setWeightMultiplier(float newWeight) {  weightMultiplier = newWeight; }
    [Tooltip("Minimum change in linear velocity before the object applies an impulse force")]
    [SerializeField] float forceApplyMin;
    [SerializeField] Vector2 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
		rb.mass = weightMultiplier * rb.mass;
        lastVelocity = rb.linearVelocity;
	}

    private void Update()
    {
        ApplyForce();
    }

    public void ApplyForce()
    {
       /* Vector2 curVelocity = rb.linearVelocity;
        Vector2 minVelocityTarget = new Vector2(lastVelocity.x + forceApplyMin, lastVelocity.y + forceApplyMin);
        if(minVelocityTarget.x <= curVelocity.x || minVelocityTarget.y <= curVelocity.y)
        {
            //Debug.Log("Min Velocity is " + minVelocityTarget);
            rb.AddForce(curVelocity, ForceMode2D.Impulse);
            //Debug.Log("Applying Force Of" + curVelocity);
        }
        lastVelocity = curVelocity;*/
    }

    public float CalcForceDamage(Collision2D collision)
    { 
        float forceDamage = forceDamageMult * Mathf.Abs(this.gameObject.GetComponent<Rigidbody2D>().linearVelocityX); //Need Angle of velocity 
        float impactSpeed = collision.relativeVelocity.magnitude;
        float mass = collision.rigidbody.mass;

        forceDamage = impactSpeed * mass;
        Debug.Log("applying " + Mathf.Max(forceDamageMin, forceDamage));
        // Need X and Y
        //Need mass
        //Apply Force
        if(forceDamage > forceDamageThreshold)
        {
            return Mathf.Max(forceDamageMin, forceDamage);
        }
        return 0;
    }

    public Vector2 CalcForce()
    {
        Vector2 force = Vector2.zero;
        float forceDamage = forceDamageMult * Mathf.Abs(this.gameObject.GetComponent<Rigidbody2D>().linearVelocityX); //Need Angle of velocity 
        // Need X and Y
        //Need mass
        //Apply Force
        return force;

    }
}
