using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class Balloon : MonoBehaviour
{
    [SerializeField] List<GameObject> touchObjects = new List<GameObject>();   
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Block>())
        {
            foreach(GameObject obj in touchObjects)
            {
                obj.GetComponent<CustomPhysics>().TogglePhysics(true);
                obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            }
            touchObjects.Clear();
            Destroy(this.gameObject);
        }

        if(collision.gameObject.GetComponent<CustomPhysics>())
        {
            collision.gameObject.GetComponent<CustomPhysics>().TogglePhysics(false);
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            touchObjects.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CustomPhysics>())
        {
            Debug.Log(collision.gameObject);
            //collision.gameObject.GetComponent<Rigidbody2D>().simulated = true;
            //touchObjects.Remove(collision.gameObject);
        }
    }

}
