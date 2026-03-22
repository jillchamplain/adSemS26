using System.Collections;
using UnityEngine;

public abstract class CustomPhysics : MonoBehaviour
{
    [Header("Custom Physics")]
    [SerializeField] bool activePhysics = false;
    public void TogglePhysics(bool physics) {  activePhysics = physics; }
    [SerializeField] protected Rigidbody2D rb;

    [Tooltip("Minimum damage done on collision")]
    [SerializeField] static float damageMin = 1;

    [SerializeField] protected Vector2 lastVelocity;
    [Tooltip("Linear velocity of last frame")]
    [SerializeField] float terminalVelocity;
    [Tooltip("Minimum velocity to destroy this on collision")]

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = rb.mass;
        lastVelocity = rb.linearVelocity;
    }

    virtual public void Start()
    {
        StartCoroutine(EnablePhysicsAfter(.1f));
    }
    private void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity * Time.deltaTime;
    }

    IEnumerator EnablePhysicsAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);   
        activePhysics = true;
    }

    public void ApplyForce(Vector2 force)
    {
        rb.AddRelativeForce(force);
    }

    public Vector2 CalcForce(Collision2D collision)
    {
        Vector2 force = Vector2.zero;

        Vector2 normal = collision.contacts[0].normal;
        Vector2 sendingVelocity = collision.relativeVelocity;
        Vector2 reflectedDirection = Vector2.Reflect(sendingVelocity, normal);

        force = reflectedDirection;
        //Debug.Log("Force applied is " +  force);
        return force;

    }

    protected float CalcDamage(Vector2 reachedVelocity)
    {
        Debug.Log(reachedVelocity);
        float damage = 0;
        float reachedX = Mathf.Abs(reachedVelocity.x);
        float reachedY = Mathf.Abs(reachedVelocity.y);
        damage = Mathf.Max(damageMin, reachedX, reachedY);

        if (reachedX >= terminalVelocity ||  reachedY >= terminalVelocity)
        {
            if (GetComponent<IDamageable>() != null)
                GetComponent<IDamageable>().Destruct();
        }
        return Mathf.Max(damage, reachedX, reachedY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!activePhysics)
            return;
        float damage = CalcDamage(lastVelocity);
        if(GetComponent<IDamageable>() != null)
            GetComponent<IDamageable>().TakeDamage(damage);

        if (collision.gameObject.GetComponent<IDamageable>() != null)
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);

        if(collision.gameObject.GetComponent<CustomPhysics>() != null)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            collision.gameObject.GetComponent<CustomPhysics>().ApplyForce(CalcForce(collision));
        }
    }

}
