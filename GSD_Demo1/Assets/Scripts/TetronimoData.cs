using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//add Long L to enum list
public enum Tetronimo {I, O, T, J, L, S, Z, longL}

[Serializable]
public struct TetronimoData
{
    public Tetronimo tetronimo;
    public Vector2Int[] cells;
    public Tile tile;
}
