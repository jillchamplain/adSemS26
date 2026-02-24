using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
public class BlockManager : MonoBehaviour, ISubManager
{
    [HideInInspector] public static BlockManager instance;
    [Header("References")]
    [SerializeField] GameObject blockParentPF;
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
        GameObject newBlock = Instantiate(blockParentPF);
        //make smaller block sections and attack to newBlock
        foreach(GridPiece pos in matchPieces)
        {
            GameObject newBlockpiece = Instantiate(blockPartPF, pos.transform.position, Quaternion.identity);
            newBlockpiece.transform.parent = newBlock.transform;

            newBlock.GetComponent<Block>().getSpriteRenderers().Add(newBlockpiece.GetComponentInChildren<SpriteRenderer>());
        }
        newBlock.GetComponent<Block>().setMatchItemType(type);
        newBlock.GetComponent<Block>().setMatchShapeType(shape);
        newBlock.GetComponent<Block>().setSpritesTo(getSpriteOfType(type));
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
