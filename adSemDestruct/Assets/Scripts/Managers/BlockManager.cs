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
        foreach (GameObject block in blocks)
        {
            if (block.GetComponent<Block>() && block.GetComponent<Block>().getMatchShapeType() == type)
            {
                return block;
            }
        }
        return null;
    }
    [Header("References")]
    [SerializeField] GameObject blockPartPF;
    [SerializeField] List<Sprite> blockPartSprites;
    Sprite getSpriteOfType(MatchItemType type)
    {
        return blockPartSprites[(int)type];
    }

    

    // Update is called once per frame

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    #region EVENTS

    public delegate void BlockCreated();
    public static event BlockCreated blockCreated;
    private void OnEnable()
    {
        MatchGrid.match += TestSpawn;
    }

    private void OnDisable()
    {
        MatchGrid.match -= TestSpawn;
    }
    #endregion

    public void TestSpawn(List<GridPiece> matchPieces, Vector3 originPos, MatchShapeType shape, MatchItemType type)
    {
        GameObject newBlock = new GameObject();
        //make smaller block sections and attack to newBlock
        foreach(GridPiece pos in matchPieces)
        {
            GameObject newBlockpiece = Instantiate(blockPartPF, pos.transform.position, Quaternion.identity);
            newBlockpiece.transform.parent = newBlock.transform;
        }
        newBlock.AddComponent<Rigidbody2D>();
        newBlock.AddComponent<Block>();
        newBlock.GetComponent<Block>().setMatchItemType(type); 
        newBlock.GetComponent<Block>().setSpritesTo(getSpriteOfType(type));
        newBlock.AddComponent<CompositeCollider2D>();
        //blockCreated?.Invoke();
    }

    void SpawnBlockCall(List<GridPiece> matchPieces, Vector3 origin, MatchShapeType shape, MatchItemType type)
    {
        StartCoroutine(SpawnBlock(matchPieces, origin, shape, type));
    }

    IEnumerator SpawnBlock(List<GridPiece> matchPieces, Vector3 origin, MatchShapeType shape, MatchItemType type)
    {
        yield return new WaitForSeconds(.5f);
        //GameObject newBlock = Instantiate(getBlockOfShape(shape), origin, Quaternion.identity); //Need to find position
        //newBlock.GetComponent<Block>().setMatchItemType(type);
        //blocks.Add(newBlock);
        

    }

    #region ISubManager
    public void HandleGameState(GameState state)
    {

    }
    #endregion
}
