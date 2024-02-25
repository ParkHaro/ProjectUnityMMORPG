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
        Debug.Log("Leave");
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
        Managers.Object.RemoveMyPlayer();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("Spawn");
        S_Spawn spawnPacket = packet as S_Spawn;
        Debug.Log($"SpawnPacket Players Count : {spawnPacket.Players.Count}");
        foreach (var player in spawnPacket.Players)
        {
            Managers.Object.Add(player, isMyPlayer: false);
        }

        Debug.Log($"{nameof(S_SpawnHandler)} / {spawnPacket.Players}");
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (var playerId in despawnPacket.PlayerIds)
        {
            Managers.Object.Remove(playerId);
        }

        Debug.Log($"{nameof(S_DespawnHandler)}");
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        var go = Managers.Object.FindById(movePacket.PlayerId);
        if (go == null)
        {
            return;
        }
        
        var creatureController = go.GetComponent<CreatureController>();
        if (creatureController == null)
        {
            return;
        }
        
        creatureController.PosInfo = movePacket.PosInfo;
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;
        
        var go = Managers.Object.FindById(skillPacket.PlayerId);
        if (go == null)
        {
            return;
        }
        
        var playerController = go.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.UseSkill(skillPacket.Info.SkillId);
        }
    }
}