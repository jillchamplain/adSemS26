using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class BlockManager : MonoBehaviour
{
    [HideInInspector] public static BlockManager instance;
    [SerializeField] List<GameObject> blocks;
    public GameObject getBlockOfShape(BlockShape type)
    {
        foreach(GameObject block in blocks)
        {
            if(block.GetComponent<Block>() && block.GetComponent<Block>().getShape() == type)
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
        MatchGrid.match += SpawnBlock;
    }

    private void OnDisable()
    {
        MatchGrid.match -= SpawnBlock;
    }

    void SpawnBlock(List<GridPiece> matchPieces, Vector3 origin, BlockShape shape, MatchItemType type)
    {
        GameObject newBlock = Instantiate(getBlockOfShape(shape), origin, Quaternion.identity); //Need to find position
        newBlock.GetComponent<Block>().setMatchItemType(type);
    }
}
