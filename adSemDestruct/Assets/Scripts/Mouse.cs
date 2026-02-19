using Unity.VisualScripting;
using UnityEngine;

public class Mouse : MonoBehaviour, ISubManager
{
    bool shouldInteract = true;
    [SerializeField] LayerMask interactMask;
    [SerializeField] GameObject held;

    #region EVENTS
    private void OnEnable()
    {
        GameManager.gameStateChangeTo += HandleGameState;
    }

    private void OnDisable()
    {
        GameManager.gameStateChangeTo -= HandleGameState;
    }
    #endregion
    void Update()
    {
        if (shouldInteract)
        {
            if (!ClickCheck())
                HoldCheck();

            ReleaseCheck();
        }
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

    void HandleMouseState(GameState state)
    {
        switch(state)
        {
            case GameState.PLAY:
                shouldInteract = true; 
                break;
            case GameState.MENU:
            case GameState.PAUSED:
            case GameState.GAME_OVER:
            case GameState.GAME_WIN:
                shouldInteract = false;
                break;
        }
    }

    #region ISubManager

    public void HandleGameState(GameState state)
    {
        HandleMouseState(state);
    }

    #endregion
}

