using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{

    public TetronimoData[] tetronimos;

    Piece activePiece;
    public Piece prefabPiece;

    public Tilemap tilemap;
    public Vector2Int boardSize;
    public Vector2Int startPosition;

    public TetrisManager tM;

    public float dropInterval = 0.5f;

    float dropTime = 0.0f;

    Dictionary<Vector3Int, Piece> pieces = new Dictionary<Vector3Int, Piece>();

    int left
    {
        get { return -boardSize.x / 2; }
    }    
    int right
    {
        get { return boardSize.x / 2; }
    }    
    int top
    {
        get { return boardSize.y / 2; }
    }    
    int bottom
    {
        get { return -boardSize.y / 2; }
    }

    public void Update()
    {
        if (tM.gameOver) return;
    
        dropTime += Time.deltaTime;

        if (dropTime >= dropInterval )
        {
            dropTime = 0.0f;

            Clear(activePiece);

            bool moveResult = activePiece.Move(Vector2Int.down);
            Set(activePiece);

            if (!moveResult)
            {
                activePiece.freeze = true;
                CheckBoard();
                SpawnPiece();
            }
        }
    }

    public void SpawnPiece()
    {
        activePiece = Instantiate(prefabPiece);
        Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);

        activePiece.Initialize(this, t);

        CheckEndGame();

        Set(activePiece);
    }

    void CheckEndGame()
    {
        if (!IsPositionValid(activePiece, activePiece.position))
        {
            tM.SetGameOver(true);
        }
    }

    public void UpdateGameOver()
    {
        if (!tM.gameOver)
        {
            ResetBoard();
        }
    }

    public void ResetBoard()
    {
        Piece[] foundPieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);

        foreach (Piece piece in foundPieces)
        {
            Destroy(piece.gameObject);
        }

        activePiece = null;

        tilemap.ClearAllTiles();

        pieces.Clear();

        SpawnPiece();
    }

    void SetTile(Vector3Int cellPos, Piece piece)
    {
        if (piece == null)
        {
            tilemap.SetTile(cellPos, null);
            pieces.Remove(cellPos);
        }
        else
        {
            tilemap.SetTile(cellPos, piece.data.tile);
            pieces[cellPos] = piece;
        }
    }
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, piece);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, null);
        }
    }

    public bool IsPositionValid(Piece piece, Vector2Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + position);

            if (cellPosition.x < left || cellPosition.x >= right || cellPosition.y < bottom || cellPosition.y >= top) return false;

            if (tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    bool IsLineFull (int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);
            if (!tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    void DestroyLine(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);

            if (pieces.ContainsKey(cellPosition))
            {
                Piece piece = pieces[cellPosition];

                piece.ReduceActiveCount();
                SetTile(cellPosition, null);
            }
        }
    }

    void ShiftRowsDown(int clearedRow)
    {
        for (int y = clearedRow + 1; y < top; y++)
        {
            for (int x = left; x < right; x++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);

                if (pieces.ContainsKey(cellPosition))
                {
                    Piece currentPiece = pieces[cellPosition];

                    SetTile(cellPosition, null);

                    cellPosition.y -= 1;
                    SetTile(cellPosition, currentPiece);
                }
            }
        }
    }
    public void CheckBoard()
    {
        List<int> destroyedLines = new List<int>();

        for (int y = bottom; y < top; y++)
        {
            if (IsLineFull(y))
            {
                DestroyLine(y);
                destroyedLines.Add(y);
            }
        }
        int rowsShifted = 0;
        foreach (int y in destroyedLines)
        {
            ShiftRowsDown(y - rowsShifted);
            rowsShifted++;
        }

        int score = tM.CalculateScore(destroyedLines.Count);

        tM.ChangeScore(score);
    }


}
