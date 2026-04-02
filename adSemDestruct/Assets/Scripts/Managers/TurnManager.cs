using DG.Tweening;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour, ISubManager
{
    [HideInInspector] public static TurnManager instance;
    [SerializeField] int turnsLeft;
    public int getTurnsLeft() {  return turnsLeft; }

    [Header("References")]
    [SerializeField] GameObject turnUI;
    [SerializeField] TextMeshProUGUI turnsTF; //Gonna need UI manager
    [SerializeField] GameObject turnParticlePF; 

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
        turnsTF.text = turnsLeft.ToString();
    }

    void TakeTurn()
    {
        turnsLeft--;

        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(turnUI.transform.position);
        spawnPos = new Vector3(spawnPos.x, spawnPos.y, 0);
        GameObject turnParticle = Instantiate(turnParticlePF, spawnPos, Quaternion.identity);
        turnParticle.GetComponentInChildren<TextMeshPro>().DOFade(0f, .5f);
        turnParticle.transform.DOLocalMoveY(-1f, 5f);
        //Debug.Log($"Taking turn, should spawn at {Camera.main.ScreenToWorldPoint(turnUI.transform.position)}");

        UpdateUI();

        if (turnsLeft <= 0)
        {
            reachedMaxTurns?.Invoke();
        }

    }

    #region ISubManager
    public void HandleGameState(GameState state)
    {

    }
    #endregion
}
