using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameRoom
    {
        private object _lock = new object();
        public int RoomId { get; set; }
        
        private readonly List<Player> _players = new List<Player>();

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
            {
                return;
            }

            lock (_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;

                // 본인에게 정보 전송
                {
                    var enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);
                    
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (var player in _players)
                    {
                        if (newPlayer != player)
                        {
                            spawnPacket.Players.Add(player.Info);
                        }
                    }
                    newPlayer.Session.Send(spawnPacket);
                }
                
                // 다른 플레이어에게 정보 전송
                {
                    var spawnPacket = new S_Spawn();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach (var player in _players)
                    {
                        if (newPlayer != player)
                        {
                            player.Session.Send(spawnPacket);
                        }
                    }
                }
            }
        }

        public void LeaveGame(int playerId)
        {
            lock (_lock)
            {
                var leavePlayer = _players.Find(p => p.Info.PlayerId == playerId);
                if (leavePlayer == null)
                {
                    return;
                }

                _players.Remove(leavePlayer);
                leavePlayer.Room = null;
                
                // 본인에게 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    leavePlayer.Session.Send(leavePacket);
                }
                
                // 다른 플레이어에게 정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.PlayerIds.Add(leavePlayer.Info.PlayerId);
                    foreach (var player in _players)
                    {
                        if (leavePlayer != player)
                        {
                            player.Session.Send(despawnPacket);
                        }
                    }
                }
            }
        }
    }
}