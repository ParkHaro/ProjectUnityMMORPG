using System;
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

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (var obj in _objects)
        {
            if (condition.Invoke(obj))
            {
                return obj;
            }
        }

        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }
}