using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class Balloon : MonoBehaviour
{
    [SerializeField] List<GameObject> touchObjects = new List<GameObject>();   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(touchObjects.Count <= 0)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            touchObjects.Add(collision.gameObject);
        }

        else
        {
            foreach (GameObject obj in touchObjects)
            {
                if(obj != null)
                    obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            }
            touchObjects.Clear();
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (touchObjects.Count <= 0)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            touchObjects.Add(collision.gameObject);
        }

        else
        {
            foreach (GameObject obj in touchObjects)
            {
                if (obj != null)
                    obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            }
            touchObjects.Clear();
            this.GetComponent<CircleCollider2D>().isTrigger = true;
            Destroy(this.gameObject);
        }
    }

}
