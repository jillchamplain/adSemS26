using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    public GridPiece getGridPieceAt(int x, int y) 
    {  
        if(x  < 0 || x >= rows)
            return null;
        if(y < 0 || y >= columns)
            return null;
        return gridPieces[x, y]; 
    }

    [Header("References")]
    [SerializeField] GameObject gridPiecePF;

    public delegate void Match(List<GridPiece> matchPieces);
    public static event Match match;

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

    #region MATCH RECOGNITION
    bool MatchRecognition(MatchItem item) //Return shape??? return grid positions?
    {
        int x = item.row;
        int y = item.col;

        bool isMatch = false;
        bool vertMatch = VerticalMatchRecognition(item);
        bool horMatch = HorizontalMatchRecognition(item);

        //Debug.Log("vert match:" + vertMatch);
        //Debug.Log("hor match: " + horMatch);

        if (vertMatch)
        {
            isMatch = true;
            match?.Invoke(VerticalMatchCollection(item));
        }
        if(horMatch)
        {
            isMatch = true;
            match?.Invoke(HorizontalMatchCollection(item));

        }
        //Debug.Log(isMatch);

        return isMatch;
    }
    #region VERTICAL MATCHES
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

    List<GridPiece> VerticalMatchCollection(MatchItem item)
    {
        List<GridPiece> matchPieces = new List<GridPiece>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns - 2; j++) //Prevents index errors
            {

                if (gridPieces[i, j].getMatchItem() == null || gridPieces[i, j + 1].getMatchItem() == null || gridPieces[i, j + 2].getMatchItem() == null)
                {
                    continue;
                }

                if (gridPieces[i, j].getMatchItem().getType() == item.getType()
                    && gridPieces[i, j + 1].getMatchItem().getType() == item.getType() && gridPieces[i, j + 2].getMatchItem().getType() == item.getType())
                {
                    matchPieces.Add(gridPieces[i, j]);
                    matchPieces.Add(gridPieces[i, j + 1]);
                    matchPieces.Add(gridPieces[i, j + 2]);
                }
            }
        }
        return matchPieces;
    }
    #endregion

    #region HORIZONTAL MATCHES
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

    List<GridPiece> HorizontalMatchCollection(MatchItem item)
    {
        List<GridPiece> matchPieces = new List<GridPiece>();
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
                    matchPieces.Add(gridPieces[i, j]);
                    matchPieces.Add(gridPieces[i + 1, j]);
                    matchPieces.Add(gridPieces[i + 2, j]);
                }
            }
        }
        return matchPieces;
    }
    #endregion

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
                    SwapGridAssign(item, gridPieces[i, j]);

                    
                }
            }
        }

        MatchRecognition(item);
    }

    void SwapGridAssign(MatchItem theItem, GridPiece thePiece)
    {
        
        if (thePiece.getMatchItem())
        {
            //Debug.Log("swapping");
            if (getGridPieceAt(theItem.row, theItem.col) != null) //If piece is currently on the grid
            {
                Debug.Log("swapping to " + getGridPieceAt(theItem.row, theItem.col));
                GridPiece prevGridPiece = getGridPieceAt(theItem.row, theItem.col);

                //Swaps position of match item currently on grid piece
                MatchItem curMatchItem = thePiece.getMatchItem();

                //curMatchItem.setPrevRow(curMatchItem.row);
                //curMatchItem.setPrevCol(curMatchItem.col);

                prevGridPiece.setMatchItem(curMatchItem);
                //curMatchItem.row = prevGridPiece.row;
                //curMatchItem.col = prevGridPiece.col;


                curMatchItem.gameObject.transform.parent = prevGridPiece.gameObject.transform;
                curMatchItem.gameObject.transform.position = prevGridPiece.gameObject.transform.position;
            }
            else
            {
                Debug.Log(getGridPieceAt(theItem.row, theItem.col));
            }
        }

        theItem.setPrevRow(theItem.row);
        theItem.setPrevCol(theItem.col);

        thePiece.setMatchItem(theItem); //Need code for swapping positions with other item
        theItem.row = thePiece.row;
        theItem.col = thePiece.col;

       
        theItem.gameObject.transform.parent = thePiece.gameObject.transform;
        theItem.gameObject.transform.position = thePiece.gameObject.transform.position;


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