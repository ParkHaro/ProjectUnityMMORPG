using System;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            Stat.Level = 1;
            Stat.Hp = 100;
            Stat.MaxHp = 100;
            Stat.Speed = 5.0f;

            State = CreatureState.Idle;
        }

        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }
        }

        private long _nextSearchTick = 0;
        private int _searchCellDist = 10;
        private int _chaseCellDist = 20;
        private Player _target;

        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
            {
                return;
            }

            _nextSearchTick = Environment.TickCount64 + 1000;

            var target = Room.FindPlayer(player =>
            {
                var dir = player.CellPos - CellPos;
                return dir.cellDistFromZero <= _searchCellDist;
            });

            if (target == null)
            {
                return;
            }

            _target = target;
            State = CreatureState.Moving;
        }

        private int _skillRange = 1;
        private long _nextMoveTick = 0;

        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
            {
                return;
            }

            var moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            Vector2Int dir = _target.CellPos - CellPos;
            var dist = dir.cellDistFromZero;
            if (dist == 0 || dist > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            var path = Room.Map.FindPath(CellPos, _target.CellPos, isCheckObjects: false);
            if (path.Count < 2 || path.Count > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            // 스킬 체크
            if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);
            BroadcastMove();
        }

        private void BroadcastMove()
        {
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }

        private long _coolTick = 0;

        protected virtual void UpdateSkill()
        {
            if (_coolTick == 0)
            {
                // 유효한 타겟인지
                if (_target == null || _target.Room != Room || _target.Hp == 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 스킬이 아직 사용 가능한지
                var dir = _target.CellPos - CellPos;
                var dist = dir.cellDistFromZero;
                var canUseSkill = dist <= _skillRange && (dir.x == 0 || dir.y == 0);
                if (canUseSkill == false)
                {
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 타게팅 방향 주시
                var lookDir = GetDirFromVec(dir);
                if (Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadcastMove();
                }

                // 데미지 판정
                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(1, out skillData);
                _target.OnDamaged(this, skillData.damage + Stat.Attack);

                // 스킬 사용 Broadcast
                S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
                skillPacket.ObjectId = Id;
                skillPacket.Info.SkillId = skillData.id;
                Room.Broadcast(skillPacket);

                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64)
            {
                return;
            }

            _coolTick = 0;
        }

        protected virtual void UpdateDead()
        {
        }
    }
}