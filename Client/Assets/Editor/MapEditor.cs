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
            var tilemapBase = Util.FindChild<Tilemap>(go, "TilemapBase", true);
            var tilemapCollision = Util.FindChild<Tilemap>(go, "TilemapCollision", true);

            using var writer = File.CreateText($"Assets/Resources/Map/{go.name}.txt");
            writer.WriteLine(tilemapBase.cellBounds.xMin);
            writer.WriteLine(tilemapBase.cellBounds.xMax);
            writer.WriteLine(tilemapBase.cellBounds.yMin);
            writer.WriteLine(tilemapBase.cellBounds.yMax);
        

            for (int y = tilemapBase.cellBounds.yMax; y >= tilemapBase.cellBounds.yMin; y--)
            {
                for (int x = tilemapBase.cellBounds.xMin; x <= tilemapBase.cellBounds.xMax; x++)
                {
                    var tile = tilemapCollision.GetTile(new Vector3Int(x, y, 0));
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