using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class MatchItemManager : MonoBehaviour
{
    [HideInInspector] public MatchItemManager instance;
    [SerializeField] List<GameObject> matchItems = new List<GameObject>();
    public GameObject getRandomMatchItem()
    {
        int randIndex = Random.Range(0, matchItems.Count);
        for(int i = 0; i < matchItems.Count; i++)
        {
            if(i  == randIndex)
                return matchItems[i];
        }
        return null;
    }

    //EVENTS
    public delegate void MatchItemSpawned(MatchItem item, int x, int y);
    public static event MatchItemSpawned matchItemSpawned;

    private void OnEnable()
    {
        MatchGrid.gridGenerated += GenerateMatchItems;
    }

    private void OnDisable()
    {
        MatchGrid.gridGenerated -= GenerateMatchItems;
    }
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMatchItems(int rows, int columns)
    {
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                GameObject newMatchItem = Instantiate(getRandomMatchItem(), new Vector3(i, j, 0), Quaternion.identity);
                matchItemSpawned?.Invoke(newMatchItem.GetComponent<MatchItem>(), i, j);
            }
        }
    }
}
