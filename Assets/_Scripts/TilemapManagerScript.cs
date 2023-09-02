using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManagerScript : MonoBehaviour
{
    [SerializeField] private Tilemap backgroundLayer;
    [SerializeField] private Tilemap backLayer;
    [SerializeField] private Tilemap collisionLayer;
    [SerializeField] private Tilemap frontLayer;
    [SerializeField] private string levelName;

    private Dictionary<string, Sprite> allTiles;
    private Dictionary<string, Tile> animatedTiles;

    public void ClearMap()
    {
        var maps = FindObjectsOfType<Tilemap>();

        foreach (var map in maps)
        {
            map.ClearAllTiles();
        }
    }

    public void SaveMap()
    {
        var level = new Level();
        level.Name = levelName;
        level.BackgroundTiles = GetTilesFromMap(backgroundLayer).ToList();
        level.BackTiles = GetTilesFromMap(backLayer).ToList();
        level.CollisionTiles = GetTilesFromMap(collisionLayer).ToList();
        level.FrontTiles = GetTilesFromMap(frontLayer).ToList();

        ScriptableObjectUtility.SaveLevel(level);

        IEnumerable<SavedTile> GetTilesFromMap(Tilemap tilemap)
        {
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    var levelTile = tilemap.GetTile<Tile>(pos);

                    yield return new SavedTile()
                    {
                        Name = levelTile.sprite.name,
                        Position = pos,
                        Tile = levelTile,
                        Orientation = TileOrientation.NoFlip
                    };
                }
            }
        }
    }

    public void LoadMap()
    {
        allTiles = new Dictionary<string, Sprite>();
        animatedTiles = new Dictionary<string, Tile>();

        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/bigfire"));
        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/collision"));
        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/capitalBG"));
        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/damage"));
        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/decor"));
        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/dome"));
        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/help"));
        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/outside"));
        AddSpritesToDictionary(Resources.LoadAll<Sprite>("Tiles/static_tiles"));

        //animated tiles
        animatedTiles.Add("flame", Resources.Load<Tile>("flame"));
        animatedTiles.Add("burnwallb", Resources.Load<Tile>("burnwallb"));
        animatedTiles.Add("burnwallc", Resources.Load<Tile>("burnwallc"));
        animatedTiles.Add("howtoslide", Resources.Load<Tile>("howtoslide"));
        animatedTiles.Add("howtoslideright", Resources.Load<Tile>("howtoslideright"));

        var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/Resources/Levels/{levelName}.asset");

        var data = textAsset.text;

        if (string.IsNullOrWhiteSpace(data))
        {
            Debug.LogError($"Error loading {levelName}.");
            return;
        }

        var layers = data.Split('[');

        if (layers.Length != 5)
        {
            Debug.LogError($"Level {levelName} is not formatted correctly!");
            return;
        }

        SetLayerTiles(layers[1], backgroundLayer);
        SetLayerTiles(layers[2], backLayer);
        SetLayerTiles(layers[3], collisionLayer);
        SetLayerTiles(layers[4], frontLayer);

        void AddSpritesToDictionary(Sprite[] sprites)
        {
            foreach (var sprite in sprites)
            {
                allTiles.Add(sprite.name.ToLower(), sprite);
            }
        }

        void SetLayerTiles(string layer, Tilemap tileMap)
        {
            foreach (var tileSpec in layer.Split('|'))
            {
                if (!string.IsNullOrEmpty(tileSpec))
                {
                    //TILE FORMAT: "|tile_name:x,y,w"
                    var parts = tileSpec.Replace("|", string.Empty).Split(':');

                    if (parts.Length != 2)
                    {
                        Debug.LogError($"Failed to parse tile: {tileSpec}");
                        continue;
                    }

                    var name = parts[0];
                    var coords = parts[1].Split(',');

                    if (allTiles.ContainsKey(name))
                    {
                        Tile tile = ScriptableObject.CreateInstance<Tile>();
                        tile.sprite = allTiles[name];
                        tileMap.SetTile(new Vector3Int(int.Parse(coords[0]), int.Parse(coords[1]), 0), tile);
                    }
                    else if (animatedTiles.ContainsKey(name))
                    {
                        tileMap.SetTile(new Vector3Int(int.Parse(coords[0]), int.Parse(coords[1]), 0), animatedTiles[name]);
                    }
                    else
                    {
                        Debug.LogError($"Tile {name} not found!");
                    }
                }
            }

            tileMap.RefreshAllTiles();
        }
    }
}

#if UNITY_EDITOR

public static class ScriptableObjectUtility
{
    public static void SaveLevel(Level level)
    {
        TextAsset text = new TextAsset(level.Serialize());

        AssetDatabase.CreateAsset(text, $"Assets/Resources/Levels/{level.Name}.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void SaveLevelFile(ScriptableLevel level)
    {
        AssetDatabase.CreateAsset(level, $"Assets/Resources/Levels/{level.name}.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); 
    }
}

#endif

public struct Level
{
    public string Name;
    public List<SavedTile> BackgroundTiles;
    public List<SavedTile> BackTiles;
    public List<SavedTile> CollisionTiles;
    public List<SavedTile> FrontTiles;

    public string Serialize()
    {
        var builder = new StringBuilder();

        builder.Append("[");

        foreach (var tile in BackgroundTiles)
        {
            builder.Append($"|{tile.Name}:{tile.Position.x},{tile.Position.y},{(int)tile.Orientation}");
        }

        builder.Append("[");

        foreach (var tile in BackTiles)
        {
            builder.Append($"|{tile.Name}:{tile.Position.x},{tile.Position.y},{(int)tile.Orientation}");
        }

        builder.Append("[");

        foreach (var tile in CollisionTiles)
        {
            builder.Append($"|{tile.Name}:{tile.Position.x},{tile.Position.y},{(int)tile.Orientation}");
        }

        builder.Append("[");

        foreach (var tile in FrontTiles)
        {
            builder.Append($"|{tile.Name}:{tile.Position.x},{tile.Position.y},{(int)tile.Orientation}");
        }

        return builder.ToString();
    }
}