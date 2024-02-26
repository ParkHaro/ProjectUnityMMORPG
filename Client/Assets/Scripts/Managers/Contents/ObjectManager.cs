using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new();

    public static GameObjectType GetObjectTypeById(int id)
    {
        var type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }
    
    public void Add(ObjectInfo info, bool isMyPlayer = false)
    {
        var objectType = GetObjectTypeById(info.ObjectId);

        if (objectType == GameObjectType.Player)
        {
            if (isMyPlayer)
            {
                var go = Managers.Resource.Instantiate("Creature/MyPlayer");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);
                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                MyPlayer.Stat = info.StatInfo;
                MyPlayer.SyncPos();
            }
            else
            {
                var go = Managers.Resource.Instantiate("Creature/Player");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);
            
                var playerController = go.GetComponent<PlayerController>();
                playerController.Id = info.ObjectId;
                playerController.PosInfo = info.PosInfo;
                playerController.Stat = info.StatInfo;
                playerController.SyncPos();
            }
        }
        else if (objectType == GameObjectType.Monster)
        {
            var go = Managers.Resource.Instantiate("Creature/Monster");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);
            
            var monsterController = go.GetComponent<MonsterController>();
            monsterController.Id = info.ObjectId;
            monsterController.PosInfo = info.PosInfo;
            monsterController.Stat = info.StatInfo;
            monsterController.SyncPos();
        }
        else if (objectType == GameObjectType.Projectile)
        {
            var go = Managers.Resource.Instantiate("Creature/Arrow");
            go.name = "Arrow";
            _objects.Add(info.ObjectId, go);
            
            var arrowController = go.GetComponent<ArrowController>();
            arrowController.Stat = info.StatInfo;
            arrowController.SyncPos();
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

    public GameObject FindById(int id)
    {
        GameObject go;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject FindCreature(Vector3Int cellPos)
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
        MyPlayer = null;
    }
}