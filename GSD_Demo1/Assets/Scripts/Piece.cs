using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Piece : MonoBehaviour
{
    public TetronimoData data;
    public Board board;
    public Vector2Int[] cells;
    public Vector2Int position;

    int activeCellCount = -1;

    public bool freeze = false;
    public void Initialize(Board board, Tetronimo tetronimo)
    {
        this.board = board;

        for (int i = 0; i < board.tetronimos.Length; i++)
        {
            if (board.tetronimos[i].tetronimo == tetronimo)
            {
                this.data = board.tetronimos[i];
                break;
            }
        }

        cells = new Vector2Int[data.cells.Length];
        for (int i = 0;i < data.cells.Length; i++) cells[i] = data.cells[i];
        
        position = board.startPosition;

        activeCellCount = cells.Length;
    }

    private void Update()
    {
        if (board.tM.gameOver) return;
        if (freeze) return;

        board.Clear(this);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A)) 
            { 
                Move(Vector2Int.left);
                
            }
            else if (Input.GetKeyDown(KeyCode.D))
            { 
                Move(Vector2Int.right);
                
            }

            if (Input.GetKeyDown(KeyCode.S)) 
            { 
                Move(Vector2Int.down);
               
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) Rotate(1);
            else if ( Input.GetKeyDown(KeyCode.RightArrow)) Rotate(-1);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            board.CheckBoard();
        }
        board.Set(this);

        if (freeze)
        {
            board.CheckBoard();
            board.SpawnPiece();
        }
    }

    void Rotate(int direction)
    {
        Vector2Int[] originalCells = new Vector2Int[cells.Length];

        for (int i = 0; i < cells.Length; i++)
        {
            originalCells[i] = cells[i];
        }

        ApplyRotation(direction);

        if (!board.IsPositionValid(this, position))
        {
            if (!TryWallKicks())
            {
                RevertRotation(originalCells);
            }
            else
            {
                Debug.Log("success");
            }
        }
        else
        {
            Debug.Log("rotated");
        }

    }
    void RevertRotation(Vector2Int[] originalCells)
    {
        for (int i = 0;i < cells.Length;i++)
        {
            cells[i] = originalCells[i];
        }
    }

    bool TryWallKicks()
    {
        List<Vector2Int> wallKickOffsets = new List<Vector2Int>
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down,
            new Vector2Int (-1, -1),
            new Vector2Int (1, -1)
        };

        //Fix wall kicks for the long L
        if (data.tetronimo == Tetronimo.I || data.tetronimo == Tetronimo.longL)
        {
            wallKickOffsets.Add(2 * Vector2Int.left);
            wallKickOffsets.Add(2 * Vector2Int.right);
        }

        foreach (Vector2Int offset in wallKickOffsets)
        {
            if (Move(offset))
            {
                return true;
            }
        } 
            
        return false;
    }

    void ApplyRotation(int direction)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 90 * direction);

        //fix rotation for the long L
        bool isSpecial = data.tetronimo == Tetronimo.I || data.tetronimo == Tetronimo.O || data.tetronimo == Tetronimo.longL;
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cellPosition = new Vector3(cells[i].x, cells[i].y);

            if (isSpecial)
            {
                cellPosition.x -= 0.5f;
                cellPosition.y -= 0.5f;
            }

            Vector3 result = rotation * cellPosition;

            if (isSpecial)
            {
                cells[i].x = Mathf.CeilToInt(result.x);
                cells[i].y = Mathf.CeilToInt(result.y);
            }
            else
            {
                cells[i].x = Mathf.RoundToInt(result.x);
                cells[i].y = Mathf.RoundToInt(result.y);
            }
        }
    }
    void HardDrop()
    {
        while (Move(Vector2Int.down))
        {

        }
        freeze = true;
        //board.SpawnPiece();
    }

    public bool Move(Vector2Int translation)
    {
        Vector2Int newPosition = position;
        newPosition += translation;

        bool positionValid = (board.IsPositionValid(this, newPosition));
        if (positionValid) position = newPosition;
        return positionValid;
       
    }

    public void ReduceActiveCount()
    {
        activeCellCount -= 1;
        if (activeCellCount <= 0 )
        {
            Destroy(gameObject);
        }
    }
}
