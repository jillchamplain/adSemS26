using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class MatchGrid : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] int rows;
    [SerializeField] int columns;
    [SerializeField] float pieceWidth;
    [SerializeField] float pieceHeight;
     GridPiece[,] gridPieces;

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
        gridPieces = new GridPiece[rows, columns];
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
                piece.row = i;
                piece.col = j;
                gridPieces[i, j] = piece;
            }
        }
    }

    #endregion

    #region GRID RECOGNITION

    bool MatchRecognition(MatchItem item) //Return shape??? return grid positions?
    {
        int x = item.row;
        int y = item.col;

        bool isMatch = false;
        bool vertMatch = VerticalMatchRecognition(item);
        bool horMatch = HorizontalMatchRecognition(item);

        Debug.Log("vert match:" + vertMatch);
        Debug.Log("hor match: " + horMatch);

        if (vertMatch || horMatch)
            isMatch = true;

        //Debug.Log(isMatch);
        return isMatch;
    }

    bool VerticalMatchRecognition(MatchItem item)
    {
        bool isVertMatch = false;
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns - 2; j++) //Prevents index errors
            {
               
                if (gridPieces[i, j].getMatchItem() == null || gridPieces[i, j + 1].getMatchItem() == null || gridPieces[i, j + 2].getMatchItem() == null)
                {
                    continue;
                }

                if (gridPieces[i, j].getMatchItem().getType() == item.getType()
                    && gridPieces[i, j + 1].getMatchItem().getType() == item.getType() && gridPieces[i, j + 2].getMatchItem().getType() == item.getType())
                {
                    isVertMatch = true;
                }
            }
        }
        return isVertMatch;
    }

    bool HorizontalMatchRecognition(MatchItem item)
    {
        bool isHorMatch = false;
        for (int i = 0; i < rows - 2; i++)
        {
            for (int j = 0; j < columns; j++) //Prevents index errors
            {

                if (gridPieces[i, j].getMatchItem() == null || gridPieces[i + 1, j].getMatchItem() == null || gridPieces[i + 2, j].getMatchItem() == null)
                {
                    continue;
                }

                if (gridPieces[i, j].getMatchItem().getType() == item.getType()
                    && gridPieces[i + 1, j].getMatchItem().getType() == item.getType() && gridPieces[i + 2, j].getMatchItem().getType() == item.getType())
                {
                    isHorMatch = true;
                }
            }
        }
        return isHorMatch;
    }
    #endregion

    void AssignToGrid(MatchItem item, GridPiece gridPiece)
    {
        //Debug.Log("Grid Piece: " + gridPiece.getRow() + "," + gridPiece.getCol());

        UnassignFromGrid(item);

        for(int i = 0; i <rows;i++)
        {
            for(int j = 0; j < columns;j++)
            {
                if (i == gridPiece.row && j == gridPiece.col)
                {
                    gridPiece.setMatchItem(item); //Need code for swapping positions with other item
                    item.gameObject.transform.parent = gridPiece.gameObject.transform;
                    item.gameObject.transform.position = gridPiece.gameObject.transform.position;
                }
            }
        }

        MatchRecognition(item);
    }

    void UnassignFromGrid(MatchItem item)
    {
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                if (gridPieces[i,j].getMatchItem() == item)
                {
                    gridPieces[i, j].setMatchItem(null);
                }
            }
        }
    }
}