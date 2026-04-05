using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collided with " + collision.gameObject);
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            //Debug.Log("destroying " + collision.gameObject);
            collision.gameObject.GetComponent<IDamageable>().Destruct();
        }
        else if (collision.gameObject.GetComponentInParent<Block>() != null)
        {
            Destroy(collision.gameObject);
        }
    }
}
