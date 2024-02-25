using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new();

    public void Add(PlayerInfo info, bool isMyPlayer = false)
    {
        if (isMyPlayer)
        {
            var go = Managers.Resource.Instantiate("Creature/MyPlayer");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);
            MyPlayer = go.GetComponent<MyPlayerController>();
            MyPlayer.Id = info.PlayerId;
            MyPlayer.PosInfo = info.PosInfo;
            MyPlayer.SyncPos();
        }
        else
        {
            var go = Managers.Resource.Instantiate("Creature/Player");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);
            
            var playerController = go.GetComponent<PlayerController>();
            playerController.Id = info.PlayerId;
            playerController.PosInfo = info.PosInfo;
            playerController.SyncPos();
        }
    }

    public void Remove(int id)
    {
        var go = FindById(id);
        if (go == null)
        {
            return;
        }
        
        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer == null)
        {
            return;
        }
        
        Remove(MyPlayer.Id);
        MyPlayer = null;
    }

    public GameObject FindById(int id)
    {
        GameObject go;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach (var obj in _objects.Values)
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
        foreach (var obj in _objects.Values)
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
        foreach (var obj in _objects.Values)
        {
            Managers.Resource.Destroy(obj);
        }
        _objects.Clear();
    }
}