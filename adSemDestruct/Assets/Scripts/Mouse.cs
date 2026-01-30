using Unity.VisualScripting;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] LayerMask interactMask;
    [SerializeField] GameObject held;
    void Update()
    {
        if (!ClickCheck())
            HoldCheck();

        ReleaseCheck();
    }

    bool ClickCheck()
    {

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, interactMask);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit)
            {
                //Debug.Log(hit.transform.gameObject);
                return true;
            }
        }
        return false;
    }

    bool HoldCheck()
    {
        if (held)
        {
            held.GetComponent<IGrabbable>().Grabbed(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //Debug.Log("Holding " + held);
            return true;
        }

        if (Input.GetMouseButton(0))
        {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, interactMask);
            if (hit)
            {
                //Debug.Log("Holding over" + hit.transform.gameObject); 

                if (!held && (hit.transform.gameObject.GetComponent<IGrabbable>() != null)) //if grabbable & not holding
                {
                    hit.transform.gameObject.GetComponent<IGrabbable>().Grabbed(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    held = hit.transform.gameObject;
                }

                return true;
            }
        }

        held = null;
        return false;
    }

    bool ReleaseCheck()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (held)
            {
                held.GetComponent<IGrabbable>().Released();
                held = null;
            }
            return true;
        }
        return false;
    }
}

