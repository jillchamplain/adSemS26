using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
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
        if (x < 0 || x >= rows)
            return null;
        if (y < 0 || y >= columns)
            return null;
        return gridPieces[x, y];
    }

    [Header("References")]
    [SerializeField] GameObject gridPiecePF;
    [SerializeField] List<MatchShape> matchShapes;
    MatchShape getMatchShapeOfType(MatchShapeType type)
    {
        for(int i = 0; i < matchShapes.Count; i++)
        {
            if (matchShapes[i].matchShapeType == type)
                return matchShapes[i];
        }
        return null;
    }

    #region EVENTS
    public delegate void GridGeneratedWithMatch(GridPiece[,] gridPieces);
    public static event GridGeneratedWithMatch gridGeneratedWithMatch;

    public delegate void GridGenerated(GridPiece[,] gridPieces);
    public static event GridGenerated gridGenerated;

    public delegate void NullGridAt(int x, int y);
    public static event NullGridAt nullGridAt;

    public delegate void Match(List<GridPiece> matchPieces, Vector3 origin, MatchShapeType shape, MatchItemType type);
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
    }

    private void OnDisable()
    {
        MatchItem.matchItemPlaced -= AssignToGrid;
        MatchItem.matchItemDestroyed -= UnassignFromGrid;

        MatchItemManager.matchItemGenerated -= GenerateAssignToGrid;
        MatchItemManager.matchItemSpawned -= AssignToGrid;
        MatchItemManager.matchItemsGenerated -= GenerateMatchRecognition;
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
        for (int i = 0; i < rows; i++) // i is row
        {
            for (int j = 0; j < cols; j++) // j is col
            {
                Vector3 pos = new Vector3(this.transform.position.x + i * pieceWidth, this.transform.position.y + j * pieceHeight, 0);
                GameObject newPiece = GameObject.Instantiate(gridPiecePF, pos, Quaternion.identity, this.gameObject.transform);

                newPiece.name = i + "_" + j;

                GridPiece piece = newPiece.GetComponent<GridPiece>();
                piece.row = i;
                piece.col = j;
                //Debug.Log(i + "_" + j);
                gridPieces[i, j] = piece;
            }
        }
        gridGenerated?.Invoke(gridPieces);
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
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    #endregion

    #region MATCH RECOGNITION

    #region GENERATE MATCH RECOGNITION

    bool RecognizeMatches()
    {
        bool isMatch = false;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GridPiece curPiece = gridPieces[i, j];
                if (curPiece.getMatchItem() != null)
                {
                    MatchItemType curType = curPiece.getMatchItem().getType();
                    if (GenerateMatchRecogntion(curPiece, curType))
                    {
                        isMatch = true;
                    }
                }
            }
        }


        return isMatch;
    }
    void GenerateMatchRecognition()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GridPiece curPiece = gridPieces[i, j];
                if (curPiece.getMatchItem() != null)
                {
                    MatchItemType curType = curPiece.getMatchItem().getType();
                    if (GenerateMatchRecogntion(curPiece, curType))
                    { 
                        gridGeneratedWithMatch?.Invoke(gridPieces);
                    }
                }
            }
        }
    }

    bool GenerateMatchRecogntion(GridPiece curPiece, MatchItemType curType)
    {
        //Debug.Log(curType);
        if (curPiece.row + 1 < rows)
        {
            if (gridPieces[curPiece.row + 1, curPiece.col].getMatchItem())
            {
                //Debug.Log("This is the match Item" + gridPieces[curPiece.row + 1, curPiece.col].getMatchItem());
                if (gridPieces[curPiece.row + 1, curPiece.col].getMatchItem().getType() == curType) //If match on the right
                {
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.SQUARE), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.L_BOTTOM_LEFT), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.L_BOTTOM_RIGHT), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.L_TOP_RIGHT), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.HORIZONTAL_5), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.HORIZONTAL_4), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.HORIZONTAL_3), curType))
                        return true;
                }
            }
        }
        if (curPiece.col + 1 < columns)
        {
            if (gridPieces[curPiece.row, curPiece.col + 1].getMatchItem())
            {
                if (gridPieces[curPiece.row, curPiece.col + 1].getMatchItem().getType() == curType)//If match above
                {
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.L_TOP_LEFT), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.VERTICAL_5), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.VERTICAL_4), curType))
                        return true;
                    if (GenerateMatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.VERTICAL_3), curType))
                        return true;
                }
            }
        }
        return false;

    }

    bool GenerateMatchRecognitionOfShape(GridPiece origin, MatchShape shape, MatchItemType type)
    {
        bool isMatch = false;
        List<GridPiece> matchPieces = new List<GridPiece>();

        foreach (Vector2Int pos in getMatchShapeOfType(shape.matchShapeType).matchPositions)
        {
            ;
            if (origin.row + pos.x < rows && origin.row + pos.x >= 0 && origin.col + pos.y < columns && origin.col + pos.y >= 0)
            {
                if (gridPieces[origin.row + pos.x, origin.col + pos.y].getMatchItem() != null)
                {
                    //Debug.Log("checking" + gridPieces[origin.row + pos.x, origin.col + pos.y]);
                    if (gridPieces[origin.row + pos.x, origin.col + pos.y].getMatchItem().getType() == type)
                    {
                        matchPieces.Add(gridPieces[origin.row + pos.x, origin.col + pos.y]);
                    }
                }
            }
        }

        if (matchPieces.Count == shape.matchPositions.Count)
        {
            //Debug.Log("Match position count of " + shape.matchShapeType + " is " + shape.matchPositions.Count);
            isMatch = true;
        }
        return isMatch;
    }
    #endregion

    #region BASE MATCH RECOGNTION
    bool MatchRecognition()
    {
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns;j++)
            {
                GridPiece curPiece = gridPieces[i, j];
                if (curPiece.getMatchItem() != null)
                {
                    MatchItemType curType = curPiece.getMatchItem().getType();
                    if(MatchRecogntion(curPiece, curType))
                        { return true; }
                }
            }
        }
        return false;
    }
    bool MatchRecogntion(GridPiece curPiece, MatchItemType curType)
    {
        //Debug.Log(curType);
        if (curPiece.row + 1 < rows)
        {
            if (gridPieces[curPiece.row + 1, curPiece.col].getMatchItem())
            {
                //Debug.Log("This is the match Item" + gridPieces[curPiece.row + 1, curPiece.col].getMatchItem());
                if (gridPieces[curPiece.row + 1, curPiece.col].getMatchItem().getType() == curType) //If match on the right
                {
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.SQUARE), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.L_BOTTOM_LEFT), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.L_BOTTOM_RIGHT), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.L_TOP_RIGHT), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.HORIZONTAL_5), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.HORIZONTAL_4), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.HORIZONTAL_3), curType))
                        return true;
                }
            }
        }
        if (curPiece.col + 1 < columns)
        {
            if (gridPieces[curPiece.row, curPiece.col + 1].getMatchItem())
            {
                if (gridPieces[curPiece.row, curPiece.col + 1].getMatchItem().getType() == curType)//If match above
                {
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.L_TOP_LEFT), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.VERTICAL_5), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.VERTICAL_4), curType))
                        return true;
                    if (MatchRecognitionOfShape(curPiece, getMatchShapeOfType(MatchShapeType.VERTICAL_3), curType))
                        return true;
                }
            }
        }
        return false;

    }

    bool MatchRecognitionOfShape(GridPiece origin, MatchShape shape, MatchItemType type)
    {
        bool isMatch = false;
        List<GridPiece> matchPieces = new List<GridPiece>();

        foreach(Vector2Int pos in getMatchShapeOfType(shape.matchShapeType).matchPositions)
        {
  ;
            if (origin.row + pos.x < rows && origin.row + pos.x >= 0 && origin.col + pos.y < columns && origin.col + pos.y >= 0)
            {
                if (gridPieces[origin.row + pos.x, origin.col + pos.y].getMatchItem() != null)
                {
                    if (gridPieces[origin.row + pos.x, origin.col + pos.y].getMatchItem().getType() == type)
                    {
                        matchPieces.Add(gridPieces[origin.row + pos.x, origin.col + pos.y]);
                    }
                }
            }
        }

        if(matchPieces.Count == shape.matchPositions.Count)
        {
            isMatch = true;
            
            match?.Invoke(matchPieces, gridPieces[shape.originPosition.x, shape.originPosition.y].transform.position, shape.matchShapeType, type);
            FallAssignCall();
        }
        return isMatch;
    }
    #endregion

    #endregion

    #region GRID ASSIGNMENT

    #region GENERATE ASSIGN

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


    #endregion

    #region ASSIGN
    void AssignToGrid(MatchItem item, int x, int y)
    {
        GridPiece gridPiece = gridPieces[x, y];
        UnassignFromGrid(item);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (i == gridPiece.row && j == gridPiece.col)
                {
                    gridPieces[i,j].setMatchItem(item);
                    item.transform.position = gridPieces[i,y].transform.position;
                }
            }
        }

        MatchRecognition();
    }
    void AssignToGrid(MatchItem item, GridPiece gridPiece) //From mouse
    {
        if (gridPiece == null)
        {
            item.transform.position = gridPieces[item.row, item.col].transform.position;
            //Debug.Log($"Resetting position to {gridPieces[item.row, item.col].transform.position}");
            MatchRecognition();
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

        MatchRecognition();
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

            if (gridPieces[theItem.row, theItem.col] != null) //If piece is currently on the grid
            {
                GridPiece prevGridPiece = gridPieces[theItem.row, theItem.col];

                //Swaps position of match item currently on grid piece
                MatchItem curMatchItem = thePiece.getMatchItem();
               

                //ANIMATION
                curMatchItem.transform.DOMove(prevGridPiece.transform.position, 0.2f);
                yield return new WaitForSeconds(0.2f);
                prevGridPiece.setMatchItem(curMatchItem);

                //curMatchItem.gameObject.transform.parent = prevGridPiece.gameObject.transform;
                //curMatchItem.gameObject.transform.position = prevGridPiece.gameObject.transform.position;
            }
            else
            {
                Debug.Log(getGridPieceAt(theItem.row, theItem.col));
            }
        }
        else
        {
            theItem.transform.position = thePiece.transform.position;
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

    void UnassignFromGrid(int x, int y)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (i == x && j == y)
                {
                    gridPieces[i, j].setMatchItem(null);
                    //Debug.Log("unassigning at " + gridPieces[i, j]);
                }
            }
        }
    }
    #endregion

    #region FALL ASSIGN
    
    void FallAssignCall()
    {
        StartCoroutine(FallAssign());
    }

    #endregion
    #endregion

    #region TESTING
    IEnumerator FallAssign()
    {
        int nullCount = 0;
        for(int i = 0; i < rows; i++)
        {
            nullCount = 0;
            for (int j = 0; j < columns; j++)
            {
                if (gridPieces[i, j].getMatchItem() == null)
                {
                    nullCount++;
                    //Debug.Log(nullCount);
                }

                else if (nullCount > 0 && j - nullCount >= 0)
                {
                    MatchItem movingItem = gridPieces[i, j].getMatchItem();
                    movingItem.transform.DOMove(gridPieces[i, j - nullCount].transform.position, .5f);
                    gridPieces[i, j - nullCount].setMatchItem(movingItem);
                    gridPieces[i, j].setMatchItem(null);
                }
            }
        }
        yield return new WaitForSeconds(.5f);
        StartCoroutine(TestLogic());
    }

    IEnumerator TestLogic()
    {
        RepopulateGridCall(rows, columns);
        yield return new WaitForSeconds(.5f);

        while (RecognizeMatches())
        {
            yield return new WaitForSeconds(0.5f);
            //Destroy any matches
        }
    }
    #endregion
}