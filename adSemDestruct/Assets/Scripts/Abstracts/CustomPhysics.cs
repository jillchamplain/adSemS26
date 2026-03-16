using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public abstract class CustomPhysics : MonoBehaviour
{
    [Header("Custom Physics")]
    [SerializeField] protected Rigidbody2D rb;
    public void setRigidBody(Rigidbody2D rb) { this.rb = rb; }
    [SerializeField] float forceDamageMult = 0;
    public void setForceDamageMult(float newMult) { forceDamageMult = newMult; } //Set these based on match type or level material type

    [Tooltip("The minimum damage this can do to an object")]
    [SerializeField] protected float forceDamageMin;
    public void setForceDamageMin(float newMin) { forceDamageMin = newMin; }
    [Tooltip("The minimum damage this needs to calculate to register damage")]
    [SerializeField] float forceDamageThreshold;

    [SerializeField] float weightMultiplier = 1;
    public void setWeightMultiplier(float newWeight) { weightMultiplier = newWeight; }
    [Tooltip("Minimum change in linear velocity before this applies an impulse force")]
    [SerializeField] float forceApplyMin;

    /*[Header("Ground Check")]
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool wasGrounded = true;*/

    [Header("Collision Damage")]
    [SerializeField] protected Vector2 lastVelocity;
    [Tooltip("Linear velocity of last frame")]
    [SerializeField] float minRegisterVelocity;
    [Tooltip("Minimum velocity this needs to reach in order to calculate collision damage")]

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = weightMultiplier * rb.mass;
        lastVelocity = rb.linearVelocity;
    }
    private void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
    }

    private void Update()
    {

    }

    public void ApplyForce(Vector2 force)
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
        rb.AddRelativeForce(force);
    }

    public float CalcForceDamage(Collision2D collision)
    {

        float forceDamageX = forceDamageMult * Mathf.Abs(this.gameObject.GetComponent<Rigidbody2D>().linearVelocityX); //Need Angle of velocity 
        float forceDamageY = forceDamageMult * Mathf.Abs(this.gameObject.GetComponent<Rigidbody2D>().linearVelocityY);
        float forceDamage = Mathf.Max(forceDamageX, forceDamageY);
        float impactSpeed = collision.relativeVelocity.magnitude;
        float mass = collision.rigidbody.mass;

        forceDamage = impactSpeed * mass;

        //Need mass
        //Apply Force
        if (forceDamage > forceDamageThreshold)
        {
            //Debug.Log("applying " + Mathf.Max(forceDamageMin, forceDamage));
            // Need X and Y
            return Mathf.Max(forceDamageMin, forceDamage);
        }
        return 0;
    }

    public Vector2 CalcForce(Collision2D collision)
    {
        Vector2 force = Vector2.zero;

        Vector2 normal = collision.contacts[0].normal;
        Vector2 sendingVelocity = collision.relativeVelocity;
        Vector2 reflectedDirection = Vector2.Reflect(sendingVelocity, normal);

        force = reflectedDirection;
        //float forceDamage = forceDamageMult * Mathf.Abs(this.gameObject.GetComponent<Rigidbody2D>().linearVelocityX); //Need Angle of velocity 
        //float mass = rb.mass;
        // Need X and Y
        //Need mass
        //Apply Force
        return force;

    }

    #region GROUND CHECKS & FALL DAMAGE
    /*bool CheckGround()
    {
        float castDistance = .01f;
        Collider2D collider = GetComponent<Collider2D>();
        float bottomEdgeY = collider.bounds.min.y; //Check ANY x
        Vector2 checkPos = new Vector2(transform.position.x, bottomEdgeY);

        RaycastHit2D boxHit;
        boxHit = Physics2D.BoxCast(checkPos, new Vector2(collider.bounds.size.x, castDistance), 0f, -transform.up, castDistance);
        if (boxHit && boxHit.collider.gameObject != this.gameObject)
        {
            Debug.Log(gameObject + " touching " + boxHit.collider);
            isGrounded = true;
        }
        else
            isGrounded = false;
        return isGrounded;
    }*/
    protected float CalcCollisionDamage(Vector2 reachedVelocity)
    {
        float damage = 0;
        float reachedX = Mathf.Abs(reachedVelocity.x);
        float reachedY = Mathf.Abs(reachedVelocity.y);
        if (reachedX >= minRegisterVelocity || reachedY >= minRegisterVelocity)
        {
            //Debug.Log("start health is " + mHealth);
            //Debug.Log(this.gameObject + "fell");
            damage = Mathf.Max(1f,reachedX, reachedY);
        }
        return damage;
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = CalcCollisionDamage(lastVelocity);
        if(GetComponent<IDamageable>() != null)
            GetComponent<IDamageable>().TakeDamage(damage);

        if (collision.gameObject.GetComponent<IDamageable>() != null)
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);

        if(collision.gameObject.GetComponent<CustomPhysics>() != null)
        {
            collision.gameObject.GetComponent<CustomPhysics>().ApplyForce(CalcForce(collision));
        }

    }

}
