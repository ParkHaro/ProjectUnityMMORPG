using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    private List<GameObject> _objects = new List<GameObject>();

    public void Add(GameObject go)
    {
        _objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        _objects.Remove(go);
    }

    public void Clear()
    {
        _objects.Clear();
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach (var obj in _objects)
        {
            var creatureController = obj.GetComponent<CreatureController>();
            if (creatureController == null)
            {
                continue;
            }

            if (creatureController.CellPos == cellPos)
            {
                return obj;
            }
        }

        return null;
    }
}