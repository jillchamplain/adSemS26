using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] float force;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>())
        {
            Debug.Log("Fan collided with " + collision.gameObject);
            Vector3 blowForce = (transform.right + transform.up) * force;
            Debug.Log(blowForce);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(blowForce);
        }
        else if (collision.gameObject.GetComponentInParent<Rigidbody2D>())
        {
            Debug.Log("Fan collided with " + collision.gameObject);
            Vector3 blowForce = (transform.right + transform.up) * force;
            Debug.Log(blowForce);
            collision.gameObject.GetComponentInParent<Rigidbody2D>().AddForce(blowForce);
        }
        
    }
}
