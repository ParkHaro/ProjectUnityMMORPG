using System;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine(
            $"C_MoveHandler : {movePacket.PosInfo.PosX} {movePacket.PosInfo.PosY} {movePacket.PosInfo.MoveDir} {movePacket.PosInfo.State}");

        var player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        var room = player.Room;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleMove, player, movePacket);
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
        C_Skill skillPacket = packet as C_Skill;
        ClientSession clientSession = session as ClientSession;

        var player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        var room = player.Room;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleSkill, player, skillPacket);
    }
}