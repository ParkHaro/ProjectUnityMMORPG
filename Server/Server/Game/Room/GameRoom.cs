using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public class GameRoom
    {
        private object _lock = new object();
        public int RoomId { get; set; }

        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private readonly Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        private readonly Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public Map Map { get; private set; } = new Map();

        public void Init(int mapId)
        {
            Map.LoadMap(mapId, "../../../../../Common/MapData");
        }

        public void Update()
        {
            lock (_lock)
            {
                foreach (var projectile in _projectiles.Values)
                {
                    projectile.Update();
                }
            }
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            var type = ObjectManager.GetObjectTypeById(gameObject.Id);

            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player newPlayer = gameObject as Player;
                    _players.Add(gameObject.Id, newPlayer);
                    newPlayer.Room = this;

                    // 본인에게 정보 전송
                    {
                        var enterPacket = new S_EnterGame();
                        enterPacket.Player = gameObject.Info;
                        newPlayer.Session.Send(enterPacket);

                        S_Spawn spawnPacket = new S_Spawn();
                        foreach (var player in _players.Values)
                        {
                            if (gameObject != player)
                            {
                                spawnPacket.Objects.Add(player.Info);
                            }
                        }

                        newPlayer.Session.Send(spawnPacket);
                    }
                }
                else if (type == GameObjectType.Monster)
                {
                    var monster = gameObject as Monster;
                    _monsters.Add(gameObject.Id, monster);
                    monster.Room = this;
                }
                else if (type == GameObjectType.Projectile)
                {
                    var projectile = gameObject as Projectile;
                    _projectiles.Add(gameObject.Id, projectile);
                    projectile.Room = this;
                }

                // 다른 플레이어에게 정보 전송
                {
                    var spawnPacket = new S_Spawn();
                    spawnPacket.Objects.Add(gameObject.Info);
                    foreach (var player in _players.Values)
                    {
                        if (player.Id != gameObject.Id)
                        {
                            player.Session.Send(spawnPacket);
                        }
                    }
                }
            }
        }

        public void LeaveGame(int objectId)
        {
            var type = ObjectManager.GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player leavePlayer;
                    if (_players.Remove(objectId, out leavePlayer) == false)
                    {
                        return;
                    }

                    leavePlayer.Room = null;
                    Map.ApplyLeave(leavePlayer);

                    // 본인에게 정보 전송
                    {
                        S_LeaveGame leavePacket = new S_LeaveGame();
                        leavePlayer.Session.Send(leavePacket);
                    }
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster monster = null;
                    if (_monsters.Remove(objectId, out monster) == false)
                    {
                        return;
                    }

                    monster.Room = null;
                    Map.ApplyLeave(monster);
                }
                else if (type == GameObjectType.Projectile)
                {
                    Projectile projectile = null;
                    if (_projectiles.Remove(objectId, out projectile) == false)
                    {
                        return;
                    }

                    projectile.Room = null;
                }

                // 다른 플레이어에게 정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.ObjectIds.Add(objectId);
                    foreach (var player in _players.Values)
                    {
                        if (player.Id != objectId)
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
                var movePosInfo = movePacket.PosInfo;
                var info = player.Info;
                if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
                {
                    if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                    {
                        return;
                    }
                }

                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                var resMovePacket = new S_Move();
                resMovePacket.ObjectId = player.Info.ObjectId;
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

            Console.WriteLine("HandleSkill");
            lock (_lock)
            {
                var info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                {
                    return;
                }

                // TODO : 스킬 사용 가능 여부 검증

                info.PosInfo.State = CreatureState.Skill;
                var resSkillPacket = new S_Skill
                {
                    ObjectId = player.Info.ObjectId,
                    Info = new SkillInfo
                    {
                        SkillId = skillPacket.Info.SkillId
                    }
                };
                Broadcast(resSkillPacket);

                Skill skillData = null;
                if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
                {
                    return;
                }

                Console.WriteLine("Use Skill : " + skillData.skillType);
                switch (skillData.skillType)
                {
                    case SkillType.SkillAuto:
                    {
                        // TODO : 데미지 판정
                        var skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                        var target = Map.Find(skillPos);
                        if (target != null)
                        {
                            Console.WriteLine("Hit GameObject !");
                        }
                    }
                        break;
                    case SkillType.SkillProjectile:
                    {
                        // TODO : Arrow
                        var arrow = ObjectManager.Instance.Add<Arrow>();
                        if (arrow == null)
                        {
                            return;
                        }

                        arrow.Owner = player;
                        arrow.Data = skillData;
                        
                        arrow.PosInfo.State = CreatureState.Moving;
                        arrow.PosInfo.MoveDir = info.PosInfo.MoveDir;
                        arrow.PosInfo.PosX = info.PosInfo.PosX;
                        arrow.PosInfo.PosY = info.PosInfo.PosY;
                        EnterGame(arrow);
                    }
                        break;
                }
            }
        }

        public void Broadcast(IMessage packet)
        {
            lock (_lock)
            {
                foreach (var player in _players.Values)
                {
                    player.Session.Send(packet);
                }
            }
        }
    }
}