using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour, ISubManager
{
    [Serializable]
    public struct UIGroup
    {
        public string name;
        public List<GameState> states;
        public bool checkStates(GameState theState)
        {
            foreach(GameState state in states)
            {
                if (state == theState)
                    return true;
            }
            return false;
        }
        public CanvasGroup canvasGroup; 
    }

    [HideInInspector] public static UIManager instance;
    [Header("Data")]
    [SerializeField] List<UIGroup> uiGroups = new List<UIGroup>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        GameManager.gameStateChangeTo += HandleUIState;
    }

    private void OnDisable()
    {
        GameManager.gameStateChangeTo -= HandleUIState;
    }

    void ToggleUIGroup(UIGroup group, bool isOn)
    {
        if (isOn)
        {
            group.canvasGroup.alpha = 1;
            group.canvasGroup.interactable = true;
            group.canvasGroup.blocksRaycasts = true;
        }
        else
        {
            group.canvasGroup.alpha = 0;
            group.canvasGroup.interactable = false;
            group.canvasGroup.blocksRaycasts = false;
        }
    }

    void GameWin()
    {

    }

    void GameOver()
    {

    }

    void Pause()
    {

    }

    void HandleUIState(GameState state)
    {
        foreach(UIGroup ui in uiGroups)
        {
            if (ui.checkStates(state))
            {
                ToggleUIGroup(ui, true);
            }
            else
                ToggleUIGroup(ui, false);
        }
    }

    #region ISubManager

    public void HandleGameState(GameState state)
    {
            HandleUIState(state);   
    }

    #endregion
}
