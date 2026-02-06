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
        bool horMatch = CheckHorMatch(x, y, item);

        Debug.Log("vert match:" + vertMatch);
        Debug.Log("hor match: " + horMatch);

        if (vertMatch || horMatch)
            isMatch = true;

        //Debug.Log(isMatch);
        return isMatch;
    }

    #endregion

    bool VerticalMatchRecognition(MatchItem item)
    {
        bool isVertMatch = false;
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns - 2; j++) //Prevents index errors
            {
                if (gridPieces[i, j].getMatchItem() == null || gridPieces[i, j + 1].getMatchItem() == null || gridPieces[i, j + 2])
                {
                  
                    continue;
                }

                if (gridPieces[i , j + 1].getMatchItem().getType() == item.getType())
                {
                    Debug.Log("above");
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

    bool CheckVertMatch(int x, int y, MatchItem item)
    {
        bool vertMatch = false;

        if (CheckVertMatchTop(x, y, item))
            vertMatch = true;
        if (CheckVertMatchCenter(x, y, item))
            vertMatch = true;
        if (CheckVertMatchBot(x, y, item))
            vertMatch = true;

        
        return vertMatch;
    }

    bool CheckVertMatchCenter(int x, int y, MatchItem item)
    {
        bool vertMatch = true;

        if (y + 1 < 0 || y + 1 >= columns) //If cannot find top or bot no match 3
        {
            return false;
        }

        if (y - 1 < 0 || y - 1 >= columns)
        {
            return false;
        }

        if (gridPieces[x, y + 1].getMatchItem() == null || gridPieces[x, y - 1].getMatchItem() == null) //If no matchitems no match
            return false;

        if (gridPieces[x, y + 1].getMatchItem() != null)
        {
            if (gridPieces[x, y + 1].getMatchItem().getType() != item.getType())
            {
                vertMatch = false;
            }
        }

        if (gridPieces[x, y - 1].getMatchItem() != null)
        {
            if (gridPieces[x, y - 1].getMatchItem().getType() != item.getType())
            {
                vertMatch = false;
            }
        }
        return vertMatch;
    }

    bool CheckVertMatchTop(int x, int y, MatchItem item)
    {
        bool vertMatch = true;

        if (y - 1 < 0 || y - 1 >= columns) //If cannot find top or bot no match 3
        {
            return false;
        }

        if (y - 2 < 0 || y - 2 >= columns)
        {
            return false;
        }

        if (gridPieces[x, y - 1].getMatchItem() == null || gridPieces[x, y - 2].getMatchItem() == null) //If no matchitems no match
            return false;

        if (gridPieces[x, y - 1].getMatchItem() != null)
        {
            if (gridPieces[x, y - 1].getMatchItem().getType() != item.getType())
            {
                vertMatch = false;
            }
        }

        if (gridPieces[x, y - 2].getMatchItem() != null)
        {
            if (gridPieces[x, y - 2].getMatchItem().getType() != item.getType())
            {
                vertMatch = false;
            }
        }
        return vertMatch;
    }

    bool CheckVertMatchBot(int x, int y, MatchItem item)
    {
        bool vertMatch = true;

        if (y + 1 < 0 || y + 2 >= columns) //If cannot find top or bot no match 3
        {
            return false;
        }

        if (y + 1 < 0 || y + 2 >= columns)
        {
            return false;
        }

        if (gridPieces[x, y + 1].getMatchItem() == null || gridPieces[x, y + 2].getMatchItem() == null) //If no matchitems no match
            return false;

        if (gridPieces[x, y + 1].getMatchItem() != null)
        {
            if (gridPieces[x, y + 1].getMatchItem().getType() != item.getType())
            {
                vertMatch = false;
            }
        }

        if (gridPieces[x, y + 2].getMatchItem() != null)
        {
            if (gridPieces[x, y + 2].getMatchItem().getType() != item.getType())
            {
                vertMatch = false;
            }
        }
        return vertMatch;
    }



    bool CheckHorMatch(int x, int y, MatchItem item)
    {
        bool horMatch = true;

        if (x + 1 < 0 || x + 1 >= rows) //If cannot find left or right no match 3
        {
            return false;
        }

        if (x - 1 < 0 || x - 1 >= rows)
        {
            return false;
        }

        if (gridPieces[x + 1, y].getMatchItem() == null || gridPieces[x - 1, y].getMatchItem() == null) //If no matchitems no match
            return false;

        if (gridPieces[x + 1, y].getMatchItem() != null)
        {
            if (gridPieces[x + 1, y].getMatchItem().getType() != item.getType())
            {
                horMatch = false;
            }
        }

        if (gridPieces[x - 1, y].getMatchItem() != null)
        {
            if (gridPieces[x - 1, y].getMatchItem().getType() != item.getType())
            {
                horMatch = false;
            }
        }
        return horMatch;
    }

    bool CheckHorMatchCenter(int x, int y, MatchItem item)
    {
        bool horMatch = true;

        if (x + 1 < 0 || x + 1 >= rows) //If cannot find left or right no match 3
        {
            return false;
        }

        if (x - 1 < 0 || x - 1 >= rows)
        {
            return false;
        }

        if (gridPieces[x + 1, y].getMatchItem() == null || gridPieces[x - 1, y].getMatchItem() == null) //If no matchitems no match
            return false;

        if (gridPieces[x + 1, y].getMatchItem() != null)
        {
            if (gridPieces[x + 1, y].getMatchItem().getType() != item.getType())
            {
                horMatch = false;
            }
        }

        if (gridPieces[x - 1, y].getMatchItem() != null)
        {
            if (gridPieces[x - 1, y].getMatchItem().getType() != item.getType())
            {
                horMatch = false;
            }
        }
        return horMatch;
    }

    bool CheckHorMatchLeft(int x, int y, MatchItem item)
    {
        bool horMatch = true;

        if (x + 1 < 0 || x + 1 >= rows) //If cannot find left or right no match 3
        {
            return false;
        }

        if (x + 2 < 0 || x + 2 >= rows)
        {
            return false;
        }

        if (gridPieces[x + 1, y].getMatchItem() == null || gridPieces[x + 2, y].getMatchItem() == null) //If no matchitems no match
            return false;

        if (gridPieces[x + 1, y].getMatchItem() != null)
        {
            if (gridPieces[x + 1, y].getMatchItem().getType() != item.getType())
            {
                horMatch = false;
            }
        }

        if (gridPieces[x + 2, y].getMatchItem() != null)
        {
            if (gridPieces[x + 2, y].getMatchItem().getType() != item.getType())
            {
                horMatch = false;
            }
        }
        return horMatch;
    }

    bool CheckHorMatchRight(int x, int y, MatchItem item)
    {
        bool horMatch = true;

        if (x + 1 < 0 || x + 1 >= rows) //If cannot find left or right no match 3
        {
            return false;
        }

        if (x - 1 < 0 || x - 1 >= rows)
        {
            return false;
        }

        if (gridPieces[x + 1, y].getMatchItem() == null || gridPieces[x - 1, y].getMatchItem() == null) //If no matchitems no match
            return false;

        if (gridPieces[x + 1, y].getMatchItem() != null)
        {
            if (gridPieces[x + 1, y].getMatchItem().getType() != item.getType())
            {
                horMatch = false;
            }
        }

        if (gridPieces[x - 1, y].getMatchItem() != null)
        {
            if (gridPieces[x - 1, y].getMatchItem().getType() != item.getType())
            {
                horMatch = false;
            }
        }
        return horMatch;
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