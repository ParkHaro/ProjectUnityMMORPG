using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("Enter");
        S_EnterGame enterGamePacket = packet as S_EnterGame;
        Managers.Object.Add(enterGamePacket.Player, isMyPlayer: true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (var obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, isMyPlayer: false);
        }

        Debug.Log($"{nameof(S_SpawnHandler)} / {spawnPacket.Objects}");
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (var playerId in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(playerId);
        }

        Debug.Log($"{nameof(S_DespawnHandler)}");
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        var go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
        {
            return;
        }
        
        var baseController = go.GetComponent<BaseController>();
        if (baseController == null)
        {
            return;
        }
        
        baseController.PosInfo = movePacket.PosInfo;
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;
        
        var go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null)
        {
            return;
        }
        
        var creatureController = go.GetComponent<CreatureController>();
        if (creatureController != null)
        {
            creatureController.UseSkill(skillPacket.Info.SkillId);
        }
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changeHpPacket = packet as S_ChangeHp;
        
        var go = Managers.Object.FindById(changeHpPacket.ObjectId);
        if (go == null)
        {
            return;
        }
        
        var creatureController = go.GetComponent<CreatureController>();
        if (creatureController != null)
        {
            creatureController.Hp = changeHpPacket.Hp;
        }
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;
        
        var go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
        {
            return;
        }
        
        var creatureController = go.GetComponent<CreatureController>();
        if (creatureController != null)
        {
            creatureController.Hp = 0;
            creatureController.OnDead();
        }
    }
}