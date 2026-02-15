using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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

    #region EVENTS
    public delegate void GridGeneratedWithMatch(int x, int y);
    public static event GridGeneratedWithMatch gridGeneratedWithMatch;

    public delegate void GridGenerated(int x, int y);
    public static event GridGenerated gridGenerated;

    public delegate void NullGridAt(int x, int y);
    public static event NullGridAt nullGridAt;

    public delegate void Match(List<GridPiece> matchPieces, Vector3 origin, BlockShape shape, MatchItemType type);
    public static event Match match;

    public delegate void MatchItemSwapped();
    public static event MatchItemSwapped matchItemSwapped;

    private void OnEnable()
    {
        MatchItem.matchItemPlaced += AssignToGrid;
        MatchItem.matchItemDestroyed += UnassignFromGrid;

        MatchItemManager.matchItemGenerated += GenerateAssignToGrid;
        MatchItemManager.matchItemSpawned += AssignToGrid;
        MatchItemManager.matchItemsGenerated += GenerateMatchRecognition;

        Block.blockCreated += ReplaceAssignCall;
        Block.blockCreated += MatchRecognition;
    }

    private void OnDisable()
    {
        MatchItem.matchItemPlaced -= AssignToGrid;
        MatchItem.matchItemDestroyed -= UnassignFromGrid;
        
        MatchItemManager.matchItemGenerated -= GenerateAssignToGrid;
        MatchItemManager.matchItemSpawned -= AssignToGrid;
        MatchItemManager.matchItemsGenerated -= GenerateMatchRecognition;

        Block.blockCreated -= ReplaceAssignCall;
        Block.blockCreated -= MatchRecognition;
    }
    #endregion
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

    void RepopulateGridCall(int rows, int cols)
    {
        StartCoroutine(RepopulateGrid(rows, cols));   
    }

    IEnumerator RepopulateGrid(int rows, int cols)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (gridPieces[i, j].getMatchItem() == null)
                {
                    nullGridAt?.Invoke(i, j);
                    //ANIMATION
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }

    #endregion

    #region MATCH RECOGNITION

    void GenerateMatchRecognition()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (gridPieces[i, j].getMatchItem() != null)
                {
                    if (GenerateMatchRecognition(gridPieces[i, j].getMatchItem()))
                    {
                        //Debug.Log("Generated with match");
                        gridGeneratedWithMatch?.Invoke(rows, columns);
                    }
                        
                }
            }
        }
    }
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

    bool GenerateMatchRecognition(MatchItem item)
    {
        int x = item.row;
        int y = item.col;

        bool isMatch = false;
        bool vertMatch = VerticalMatchRecognition(item);
        bool horMatch = HorizontalMatchRecognition(item);

        //Debug.Log("vert match:" + vertMatch);
        //Debug.Log("hor match: " + horMatch);

        if (vertMatch || horMatch)//matches of 3 
        {
            isMatch = true;
        }
        return isMatch;
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
            //Debug.Log("Vertical Match made! Type:" + item.getType());
            match?.Invoke(VerticalMatchCollection(item), FindVerticalMatchOrigin(item), BlockShape.VERTICAL, item.getType());
            
        }
        else if(horMatch)
        {
            isMatch = true;
            match?.Invoke(HorizontalMatchCollection(item), FindHorizontalMatchOrigin(item), BlockShape.HORIZONTAL, item.getType());
            //Debug.Log("Horizontal Match made! Type:" + item.getType());
        }

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

    #region GENERATE ASSIGN
    void GenerateAssignToGridCall(MatchItem item, int x, int y)
    {
       StartCoroutine(RTGenerateAssignToGrid(item, x, y));
    }

    void GenerateAssignToGrid(MatchItem item, int x, int y)
    {
        GridPiece gridPiece = gridPieces[x, y];
        UnassignFromGrid(item);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (i == gridPiece.row && j == gridPiece.col)
                {

                    gridPieces[i, j].setMatchItem(item);
                }
            }
        }
    }

    IEnumerator RTGenerateAssignToGrid(MatchItem item, int x, int y)
    {
        GridPiece gridPiece = gridPieces[x, y];
        UnassignFromGrid(item);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (i == gridPiece.row && j == gridPiece.col)
                {
                    //ANIMATION
                    item.transform.DOMove(gridPiece.transform.position, 0.1f);
                    yield return new WaitForSeconds(0.1f);

                    gridPieces[i, j].setMatchItem(item);
                }
            }
        }
    }
    #endregion

    #region ASSIGN
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
    void AssignToGrid(MatchItem item, GridPiece gridPiece) //From mouse
    {
        //Debug.Log("Grid Piece: " + gridPiece.getRow() + "," + gridPiece.getCol());
        if(gridPiece == null)
        {
            item.transform.position = gridPieces[item.row, item.col].transform.position;
            MatchRecognition(item);
            return;
        }

        UnassignFromGrid(item);

        for(int i = 0; i <rows;i++)
        {
            for(int j = 0; j < columns;j++)
            {
                if (i == gridPiece.row && j == gridPiece.col)
                {
                    SwapGridAssignCall(item, gridPieces[i, j]);

                    
                }
            }
        }

        MatchRecognition(item);
    }
    #endregion

    #region SWAP ASSIGN
    void SwapGridAssignCall(MatchItem theItem, GridPiece thePiece)
    {
        StartCoroutine(SwapGridAssign(theItem, thePiece));
    }
    IEnumerator SwapGridAssign(MatchItem theItem, GridPiece thePiece)
    {
        bool isSamePiece = false;

        if (theItem.row == thePiece.row && theItem.col == thePiece.col)
            isSamePiece = true;

        if (thePiece.getMatchItem())
        {
            theItem.gameObject.transform.position = thePiece.gameObject.transform.position;

            if (getGridPieceAt(theItem.row, theItem.col) != null) //If piece is currently on the grid
            {
                GridPiece prevGridPiece = getGridPieceAt(theItem.row, theItem.col);

                //Swaps position of match item currently on grid piece
                MatchItem curMatchItem = thePiece.getMatchItem();
               

                //ANIMATION
                curMatchItem.transform.DOMove(prevGridPiece.transform.position, 0.1f);
                yield return new WaitForSeconds(0.1f);
                prevGridPiece.setMatchItem(curMatchItem);

                //curMatchItem.gameObject.transform.parent = prevGridPiece.gameObject.transform;
                //curMatchItem.gameObject.transform.position = prevGridPiece.gameObject.transform.position;
            }
            else
            {
                Debug.Log(getGridPieceAt(theItem.row, theItem.col));
            }
        }

        theItem.setPrevRow(theItem.row);
        theItem.setPrevCol(theItem.col);


        //theItem.gameObject.transform.parent = thePiece.gameObject.transform;

        //ANIMATION
        //Debug.Log("Moving controlled piece to " + thePiece.transform.localPosition);
        //theItem.transform.DOLocalMove(thePiece.transform.localPosition, 0.5f);
        //yield return new WaitForSeconds(0.5f);

        thePiece.setMatchItem(theItem);
        theItem.row = thePiece.row;
        theItem.col = thePiece.col;

        if (!isSamePiece)
            matchItemSwapped?.Invoke();

        MatchRecognition();

    }
    #endregion

    #region UNASSIGN
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
    #endregion

    #region REPLACE ASSIGN
    
    void ReplaceAssignCall()
    {
        StartCoroutine(ReplaceAssign());
    }
    IEnumerator ReplaceAssign()
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
                            //ANIMATION
                            gridPieces[i, y].getMatchItem().transform.DOMove(gridPieces[i, j].transform.position, 0.1f);
                            yield return new WaitForSeconds(0.1f);


                            gridPieces[i, j].setMatchItem(gridPieces[i, y].getMatchItem());
                            gridPieces[i, y].setMatchItem(null);
                            break;

                        }
                    }
                }
            }
        }
        RepopulateGridCall(rows, columns);
    }
    #endregion

    #endregion
}