using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] int rows;
    [SerializeField] int columns;
    [SerializeField] float pieceWidth;
    [SerializeField] float pieceHeight;
    [SerializeField] List<GridPiece> gridPieces;

    [Header("References")]
    [SerializeField] GameObject gridPiecePF;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        Block.blockPlaced += AssignToGrid;
    }

    private void OnDisable()
    {
        Block.blockPlaced -= AssignToGrid;
    }
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateGrid()
    {
        GenerateGrid(rows, columns);
    }

    void GenerateGrid(int row, int col)
    {
        for (int i = 0; i < col; i++) // i is row
        {
            for (int j = 0; j < row; j++) // j is col
            {
                Vector3 pos = new Vector3(i * pieceWidth, j * pieceHeight, 0);
                GameObject newPiece = GameObject.Instantiate(gridPiecePF, pos, Quaternion.identity, this.gameObject.transform);

                newPiece.name = i + "_" + j;

                GridPiece piece = newPiece.GetComponent<GridPiece>();
                piece.setRow(i);
                piece.setCol(j);
                gridPieces.Add(piece);
            }
        }
    }

    void AssignToGrid(Block block, GridPiece gridPiece)
    {
        Debug.Log("Grid Piece: " + gridPiece.getRow() + "," + gridPiece.getCol());
        foreach(GridPiece piece in gridPieces)
        {
            if(piece.getRow() == gridPiece.getRow() && piece.getCol() == gridPiece.getCol())
            {
                
                block.gameObject.transform.parent = gridPiece.gameObject.transform;
                block.gameObject.transform.position = gridPiece.gameObject.transform.position;
            }
        }
    }


}
