using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public struct Pos
{
    public Pos(int y, int x)
    {
        Y = y;
        X = x;
    }

    public int Y;
    public int X;
}

public struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return F < other.F ? 1 : -1;
    }
}

public class MapManager
{
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    public int SizeX => MaxX - MinX + 1;
    public int SizeY => MaxY - MinY + 1;

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

        for (var y = 0; y < yCount; y++)
        {
            var line = reader.ReadLine();
            for (var x = 0; x < xCount; x++)
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

    #region A*

    // U D L R
    private readonly int[] _deltaY = { 1, -1, 0, 0 };
    private readonly int[] _deltaX = { 0, 0, -1, 1 };
    private int[] _cost = { 10, 10, 10, 10 };

    public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false)
    {
        var path = new List<Pos>();

        // 점수 매기기
        // F = G + H
        // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
        // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
        // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

        // (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
        var closed = new bool[SizeY, SizeX]; // CloseList

        // (y, x) 가는 길을 한 번이라도 발견했는지
        // 발견X => MaxValue
        // 발견O => F = G + H
        var open = new int[SizeY, SizeX]; // OpenList
        for (var y = 0; y < SizeY; y++)
        {
            for (var x = 0; x < SizeX; x++)
            {
                open[y, x] = Int32.MaxValue;
            }            
        }

        var parent = new Pos[SizeY, SizeX];

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        var pq = new PriorityQueue<PQNode>();

        // CellPos -> ArrayPos
        var pos = Cell2Pos(startCellPos);
        var dest = Cell2Pos(destCellPos);

        // 시작점 발견 (예약 진행)
        open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
        pq.Push(new PQNode()
            { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
        parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            var node = pq.Pop();
            // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if (closed[node.Y, node.X])
                continue;

            // 방문한다
            closed[node.Y, node.X] = true;
            // 목적지 도착했으면 바로 종료
            if (node.Y == dest.Y && node.X == dest.X)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (var i = 0; i < _deltaY.Length; i++)
            {
                var next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                // 유효 범위를 벗어났으면 스킵
                // 벽으로 막혀서 갈 수 없으면 스킵
                if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                {
                    if (CanGo(Pos2Cell(next)) == false) // CellPos
                    {
                        continue;
                    }
                }

                // 이미 방문한 곳이면 스킵
                if (closed[next.Y, next.X])
                {
                    continue;
                }

                // 비용 계산
                var g = 0; // node.G + _cost[i];
                var h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                if (open[next.Y, next.X] < g + h)
                {
                    continue;
                }

                // 예약 진행
                open[dest.Y, dest.X] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                parent[next.Y, next.X] = new Pos(node.Y, node.X);
            }
        }

        return CalcCellPathFromParent(parent, dest);
    }

    private List<Vector3Int> CalcCellPathFromParent(Pos[,] parent, Pos dest)
    {
        var cells = new List<Vector3Int>();

        var y = dest.Y;
        var x = dest.X;
        while (parent[y, x].Y != y || parent[y, x].X != x)
        {
            cells.Add(Pos2Cell(new Pos(y, x)));
            var pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }

        cells.Add(Pos2Cell(new Pos(y, x)));
        cells.Reverse();

        return cells;
    }

    private Pos Cell2Pos(Vector3Int cell)
    {
        // CellPos -> ArrayPos
        return new Pos(MaxY - cell.y, cell.x - MinX);
    }

    private Vector3Int Pos2Cell(Pos pos)
    {
        // ArrayPos -> CellPos
        return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
    }

    #endregion
}