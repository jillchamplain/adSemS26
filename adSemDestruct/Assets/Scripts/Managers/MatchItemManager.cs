using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class MatchItemManager : MonoBehaviour, ISubManager
{
    [HideInInspector] public MatchItemManager instance;
    [Header("Data")]
    [SerializeField] List<GameObject> matchItemTemplates = new List<GameObject>();
    [SerializeField] List<GameObject> matchItems = new List<GameObject>();
    public GameObject getRandomMatchItem()
    {
        int randIndex = Random.Range(0, matchItemTemplates.Count);
        for(int i = 0; i < matchItemTemplates.Count; i++)
        {
            if(i  == randIndex)
                return matchItemTemplates[i];
        }
        return null;
    }

    //EVENTS
    public delegate void MatchItemsGenerated();
    public static event MatchItemsGenerated matchItemsGenerated;

    public delegate void MatchItemGenerated(MatchItem item, int x, int y);
    public static event MatchItemGenerated matchItemGenerated;

    public delegate void MatchItemSpawned(MatchItem item, int x, int y);
    public static event MatchItemSpawned matchItemSpawned;

    private void OnEnable()
    {
        MatchGrid.gridGenerated += GenerateMatchItems;
        MatchGrid.gridGeneratedWithMatch += GenerateMatchItems;
        MatchGrid.nullGridAt += SpawnMatchItemCall;
    }

    private void OnDisable()
    {
        MatchGrid.gridGenerated -= GenerateMatchItems;
        MatchGrid.gridGeneratedWithMatch -= GenerateMatchItems;
        MatchGrid.nullGridAt -= SpawnMatchItemCall;
    }
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void ClearMatchItems()
    {
        foreach(GameObject item in matchItems)
        {
            MatchItem m = item.GetComponent<MatchItem>();
            m.DestroySelf();
            Destroy(item);
        }

        matchItems.Clear();
    }

    void GenerateMatchItems(int rows, int columns)
    {
        ClearMatchItems();
        //Debug.Log("generating match items");
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                GameObject newMatchItem = Instantiate(getRandomMatchItem(), new Vector3(i, j, 0), Quaternion.identity);
                matchItems.Add(newMatchItem);
                matchItemGenerated?.Invoke(newMatchItem.GetComponent<MatchItem>(), i, j);
            }
        }
        matchItemsGenerated?.Invoke();
    }

    void SpawnMatchItemCall(int row, int column)
    {
        StartCoroutine(SpawnMatchItem(row, column));
    }
    IEnumerator SpawnMatchItem(int row, int column)
    {
        GameObject newMatchItem = Instantiate(getRandomMatchItem(), new Vector3(row, column, 0), Quaternion.identity);
        matchItems.Add(newMatchItem);
        yield return new WaitForSeconds(0.2f);
        matchItemSpawned?.Invoke(newMatchItem.GetComponent<MatchItem>(), row, column);
    }

    #region ISubManager
    public void HandleGameState(GameState state)
    {

    }
    #endregion
}
