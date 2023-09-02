using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScriptableLevel : ScriptableObject
{
    public string LevelName;
    public List<SavedTile> BackgroundTiles;
    public List<SavedTile> BackTiles;
    public List<SavedTile> CollisionTiles;
    public List<SavedTile> FrontTiles;
}

[Serializable]
public class SavedTile
{
    public string Name;
    public Vector3Int Position;
    public TileOrientation Orientation;
    public Tile Tile;
}

public enum TileOrientation
{
    NoFlip = 0,
    FlipH = 1,
    FlipV = 2,
    FlipHV = 3
}