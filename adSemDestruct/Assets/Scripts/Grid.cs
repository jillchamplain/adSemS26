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
        MatchItem.matchItemPlaced += AssignToGrid;
    }

    private void OnDisable()
    {
        MatchItem.matchItemPlaced -= AssignToGrid;
    }
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region GRID GENERATION

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
                piece.gridBased.row = i;
                piece.gridBased.col = j;
                gridPieces.Add(piece);
            }
        }
    }

    #endregion

    #region GRID RECOGNITION

    void MatchRecognition()
    {

    }

    #endregion

    void AssignToGrid(MatchItem item, GridPiece gridPiece)
    {
        //Debug.Log("Grid Piece: " + gridPiece.getRow() + "," + gridPiece.getCol());
        foreach(GridPiece piece in gridPieces)
        {
            if(piece.gridBased.row == gridPiece.gridBased.row && piece.gridBased.col == gridPiece.gridBased.col)
            {
                
                item.gameObject.transform.parent = gridPiece.gameObject.transform;
                item.gameObject.transform.position = gridPiece.gameObject.transform.position;
            }
        }
    }


}
