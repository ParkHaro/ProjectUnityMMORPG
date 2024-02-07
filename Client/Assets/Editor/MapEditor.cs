using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
#endif

public class MapEditor
{
#if UNITY_EDITOR
    // % (Ctrl), # (Shift), & (Alt)
    [MenuItem("Tools/GenerateMap %&g")]
    private static void GenerateMap()
    {
        var gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");
        
        foreach (var go in gameObjects)
        {
            var tilemap = Util.FindChild<Tilemap>(go, "TilemapCollision", true);

            using var writer = File.CreateText($"Assets/Resources/Map/{go.name}.txt");
            writer.WriteLine(tilemap.cellBounds.xMin);
            writer.WriteLine(tilemap.cellBounds.xMax);
            writer.WriteLine(tilemap.cellBounds.yMin);
            writer.WriteLine(tilemap.cellBounds.yMax);
        

            for (int y = tilemap.cellBounds.yMax; y >= tilemap.cellBounds.yMin; y--)
            {
                for (int x = tilemap.cellBounds.xMin; x <= tilemap.cellBounds.xMax; x++)
                {
                    var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                    writer.Write(tile != null ? "1" : "0");
                }
                writer.WriteLine();
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}