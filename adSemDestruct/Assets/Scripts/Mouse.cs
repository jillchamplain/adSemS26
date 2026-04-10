using Unity.VisualScripting;
using UnityEngine;

public class Mouse : MonoBehaviour, ISubManager
{
    [SerializeField] Texture2D hoverSprite;
    [SerializeField] Texture2D clickSprite;
    [SerializeField] Texture2D holdSprite;
    Vector2 hotSpot = Vector2.zero;

    [SerializeField] bool shouldInteract = true;
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

    private void Start()
    {
        Cursor.SetCursor(hoverSprite, hotSpot, CursorMode.Auto);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Click");
            Cursor.SetCursor(clickSprite, hotSpot, CursorMode.Auto);
            //Debug.Log("Setting sprite");
        }

        else if(Input.GetMouseButton(0))
            Cursor.SetCursor(holdSprite, hotSpot, CursorMode.Auto);

        else if(Input.GetMouseButtonUp(0))
            Cursor.SetCursor(hoverSprite, hotSpot, CursorMode.Auto);


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
            Cursor.SetCursor(clickSprite, hotSpot, CursorMode.Auto);
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
            Cursor.SetCursor(holdSprite, hotSpot, CursorMode.Auto);
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
            Cursor.SetCursor(hoverSprite, hotSpot, CursorMode.Auto);
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

