using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private TileBase tileBase;

    private void Awake()
    {
        TryGetComponent(out tilemap);
    }

    private void Start()
    {
        tilemap.SetTile(Vector3Int.zero, tileBase);
    }

    private void Update()
    {
        List<Vector3Int> blocked = new List<Vector3Int>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(pos);
            if (tile)
            {
                blocked.Add(pos);
            }
        }
    }
}
