using System.Collections.Generic;
using Google.Protobuf;
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

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
            {
                return;
            }

            lock (_lock)
            {
                // TODO : 검증

                var info = player.Info;
                info.PosInfo = movePacket.PosInfo;

                var resMovePacket = new S_Move();
                resMovePacket.PlayerId = player.Info.PlayerId;
                resMovePacket.PosInfo = movePacket.PosInfo;

                Broadcast(resMovePacket);
            }
        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
            {
                return;
            }

            lock (_lock)
            {
                var info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                {
                    return;
                }
                
                // TODO : 스킬 사용 가능 여부 검증
                
                info.PosInfo.State = CreatureState.Skill;
                S_Skill resSkillPacket = new S_Skill();
                resSkillPacket.PlayerId = player.Info.PlayerId;
                resSkillPacket.SkillInfo.SkillId = 1;
                
                Broadcast(resSkillPacket);
                
                // TODO : 데미지 판정
                
            }
        }

        public void Broadcast(IMessage packet)
        {
            lock (_lock)
            {
                foreach (var player in _players)
                {
                    player.Session.Send(packet);
                }
            }
        }
    }
}