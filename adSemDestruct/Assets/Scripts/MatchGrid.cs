using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEditor.Progress;

public class MatchGrid : MonoBehaviour
{
    [HideInInspector] public static MatchGrid instance;
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

    //EVENTS
    public delegate void GridGenerated(int x, int y);
    public static event GridGenerated gridGenerated;

    public delegate void NullGridAt(int x, int y);
    public static event NullGridAt nullGridAt;

    public delegate void Match(List<GridPiece> matchPieces, Vector3 origin, BlockShape shape, MatchItemType type);
    public static event Match match;

    private void OnEnable()
    {
        MatchItem.matchItemPlaced += AssignToGrid;
        
        MatchItemManager.matchItemSpawned += AssignToGrid;

        Block.blockCreated += ReplaceAssign;
        Block.blockCreated += MatchRecognition;
    }

    private void OnDisable()
    {
        MatchItem.matchItemPlaced -= AssignToGrid;
        
        MatchItemManager.matchItemSpawned -= AssignToGrid;

        Block.blockCreated -= ReplaceAssign;
        Block.blockCreated -= MatchRecognition;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
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

    void GenerateGrid(int rows, int cols)
    {
        for (int i = 0; i < cols; i++) // i is row
        {
            for (int j = 0; j < rows; j++) // j is col
            {
                Vector3 pos = new Vector3(this.transform.position.x + i * pieceWidth, this.transform.position.y + j * pieceHeight, 0);
                GameObject newPiece = GameObject.Instantiate(gridPiecePF, pos, Quaternion.identity, this.gameObject.transform);

                newPiece.name = i + "_" + j;

                GridPiece piece = newPiece.GetComponent<GridPiece>();
                piece.row = i;
                piece.col = j;
                gridPieces[i, j] = piece;
            }
        }
        gridGenerated?.Invoke(rows, cols);
    }

    void RepopulateGrid(int rows, int cols)
    {
        for(int i = 0; i < rows; i++)
        {
            for(int j  = 0; j < cols; j++)
            {
                if (gridPieces[i, j].getMatchItem() == null)
                {
                    nullGridAt?.Invoke(i, j);
                }
            }
        }    
    }

    #endregion

    #region MATCH RECOGNITION
    void MatchRecognition()
    {
        for(int i = 0; i < rows; i++)
        {
            for(int  j = 0; j < columns; j++)
            {
                if (gridPieces[i,j].getMatchItem() != null)
                {
                    MatchRecognition(gridPieces[i, j].getMatchItem());
                }
            }
        }
    }
    bool MatchRecognition(MatchItem item) 
    {
        int x = item.row;
        int y = item.col;

        bool isMatch = false;
        bool vertMatch = VerticalMatchRecognition(item);
        bool horMatch = HorizontalMatchRecognition(item);

        //Debug.Log("vert match:" + vertMatch);
        //Debug.Log("hor match: " + horMatch);

        if (vertMatch)//matches of 3 
        {
            isMatch = true;
            match?.Invoke(VerticalMatchCollection(item), FindVerticalMatchOrigin(item), BlockShape.VERTICAL, item.getType());
            
        }
        else if(horMatch)
        {
            isMatch = true;
            match?.Invoke(HorizontalMatchCollection(item), FindHorizontalMatchOrigin(item), BlockShape.HORIZONTAL, item.getType());
            
        }
        if (isMatch)
            Debug.Log("Match made!");

        return isMatch;
    }

    List<GridPiece> SearchForMatch(MatchItem item)
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
                   
                }
            }
        }
        return matchPieces;
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
                    //Debug.Log(matchPieces);
                    return matchPieces;
                }
            }
        }
        return matchPieces;
    }

    Vector3 FindVerticalMatchOrigin(MatchItem item)
    {
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
                    return gridPieces[i, j + 1].gameObject.transform.position;
                }
            }
        }
        return Vector3.zero;
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
                    return matchPieces;
                }
            }
        }
        return matchPieces;
    }
    Vector3 FindHorizontalMatchOrigin(MatchItem item)
    {
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
                    return gridPieces[i + 1, j].gameObject.transform.position;
                }
            }
        }
        return Vector3.zero;
    }
    #endregion

    #endregion

    #region GRID ASSIGNMENT
    void AssignToGrid(MatchItem item, int x, int y)
    {
        //Debug.Log("Grid Piece: " + gridPiece.getRow() + "," + gridPiece.getCol());

        GridPiece gridPiece = gridPieces[x, y];
        UnassignFromGrid(item);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (i == gridPiece.row && j == gridPiece.col)
                {
                    gridPieces[i,j].setMatchItem(item);
                }
            }
        }

        MatchRecognition(item);
    }
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
                //Debug.Log("swapping to " + getGridPieceAt(theItem.row, theItem.col));
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

        MatchRecognition();
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
    public void ReplaceAssign()
    {
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                if (gridPieces[i, j].getMatchItem() == null)
                {
                    //Debug.Log("null spot at " + gridPieces[i, j]);
                    for(int y = j + 1; y < columns; y++) //Iterate up column
                    {
                        if (gridPieces[i, y].getMatchItem() != null)
                        {
                            //Debug.Log("setting " + gridPieces[i, y].getMatchItem() + " to " + gridPieces[i, j]);
                            
                            //Debug.Log(gridPieces[i, y] + " has been set to null");

                            gridPieces[i, j].setMatchItem(gridPieces[i, y].getMatchItem());
                            gridPieces[i, y].setMatchItem(null);
                            break;

                        }
                    }
                }
            }
        }
        RepopulateGrid(rows, columns);
    }
    #endregion
}