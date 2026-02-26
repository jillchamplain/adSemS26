using UnityEngine;

public class KillZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IDestructible>() != null)
        {
            collision.GetComponent<IDestructible>().Destruct();
            //Debug.Log("Destroyed " + collision.gameObject);
        }
    }
}
