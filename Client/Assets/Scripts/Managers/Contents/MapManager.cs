using System.IO;
using UnityEngine;

public class MapManager
{
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    private bool[,] _collisions;

    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
        {
            return false;
        }

        if (cellPos.y < MinY || cellPos.y > MaxY)
        {
            return false;
        }

        var x = cellPos.x - MinX;
        var y = MaxY - cellPos.y;
        return !_collisions[y, x];
    }

    public void LoadMap(int mapId)
    {
        DestroyMap();

        var mapName = $"Map_{mapId:000}";
        var go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";

        var collision = Util.FindChild(go, "TilemapCollision", true);
        if (collision != null)
        {
            collision.SetActive(false);
        }

        CurrentGrid = go.GetComponent<Grid>();

        var textAsset = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        var reader = new StringReader(textAsset.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        var xCount = MaxX - MinX + 1;
        var yCount = MaxY - MinY + 1;
        _collisions = new bool[yCount, xCount];

        for (int y = 0; y < yCount; y++)
        {
            var line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                _collisions[y, x] = (line![x] == '1');
            }
        }
    }

    public void DestroyMap()
    {
        var map = GameObject.Find("Map");
        if (map != null)
        {
            Object.Destroy(map);
        }
    }
}