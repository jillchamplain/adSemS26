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
            }
        }
    }

    void AssignToGrid(Vector2 pos, GameObject obj)
    {
        foreach(GridPiece piece in gridPieces)
        {
            piece.getCollider().bounds.Contains(pos);
        }
    }


}
