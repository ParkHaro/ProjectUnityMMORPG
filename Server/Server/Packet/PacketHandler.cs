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

        if (clientSession.MyPlayer == null)
        {
            return;
        }

        if (clientSession.MyPlayer.Room == null)
        {
            return;
        }

        // TODO : 검증

        var info = clientSession.MyPlayer.Info;
        info.PosInfo = movePacket.PosInfo;
        
        var resMovePacket = new S_Move();
        resMovePacket.PlayerId = clientSession.MyPlayer.Info.PlayerId;
        resMovePacket.PosInfo = movePacket.PosInfo;

        clientSession.MyPlayer.Room.Broadcast(resMovePacket);
    }
}