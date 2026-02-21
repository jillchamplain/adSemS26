using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class BlockManager : MonoBehaviour, ISubManager
{
    [HideInInspector] public static BlockManager instance;
    [Header("Data")]
    [SerializeField] List<GameObject> blocks;
    public GameObject getBlockOfShape(MatchShapeType type)
    {
        foreach(GameObject block in blocks)
        {
            if(block.GetComponent<Block>() && block.GetComponent<Block>().getMatchShapeType() == type)
            {
                return block;
            }
        }
        return null;
    }

    // Update is called once per frame

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    private void OnEnable()
    {
        MatchGrid.match += SpawnBlockCall;
    }

    private void OnDisable()
    {
        MatchGrid.match -= SpawnBlockCall;
    }

    void SpawnBlockCall(List<GridPiece> matchPieces, Vector3 origin, MatchShapeType shape, MatchItemType type)
    {
        StartCoroutine(SpawnBlock(matchPieces, origin, shape, type));
    }

    IEnumerator SpawnBlock(List<GridPiece> matchPieces, Vector3 origin, MatchShapeType shape, MatchItemType type)
    {
        yield return new WaitForSeconds(.5f);
        GameObject newBlock = Instantiate(getBlockOfShape(shape), origin, Quaternion.identity); //Need to find position
        newBlock.GetComponent<Block>().setMatchItemType(type);
        blocks.Add(newBlock);
        

    }

    #region ISubManager
    public void HandleGameState(GameState state)
    {

    }
    #endregion
}
