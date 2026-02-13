using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [HideInInspector] public static TurnManager instance;
    [Header("Data")]
    [SerializeField] int maxTurns;
    public int getMaxTurns() {  return maxTurns; }
    int curTurn = 0;
    public int getCurTurns() { return curTurn; }

    [Header("References")]
    [SerializeField] TextMeshProUGUI turnTF;
    [SerializeField] TextMeshProUGUI maxTurnsTF; //Gonna need UI manager

    //EVENTS
    public delegate void ReachedMaxTurns(); //Get turnManager to wait time before deciding
    public static event ReachedMaxTurns reachedMaxTurns;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        MatchGrid.matchItemSwapped += TakeTurn;
    }

    private void OnDisable()
    {
        MatchGrid.matchItemSwapped -= TakeTurn;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        turnTF.text = curTurn.ToString();
        maxTurnsTF.text = maxTurns.ToString();
    }

    void TakeTurn()
    {
        curTurn++;

        UpdateUI();

        if (curTurn == maxTurns)
        {
            Debug.Log("game over");
            reachedMaxTurns?.Invoke();
        }

    }
}
