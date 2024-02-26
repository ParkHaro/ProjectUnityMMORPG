using System;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;

        public int Id
        {
            get => Info.ObjectId;
            set => Info.ObjectId = value;
        }
        
        public GameRoom Room { get; set; }

        public ObjectInfo Info { get; set; } = new ObjectInfo() { PosInfo = new PositionInfo() };
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo Stat { get; private set; } = new StatInfo();

        public float Speed
        {
            get => Stat.Speed;
            set => Stat.Speed = value;
        }
        
        public int Hp
        {
            get => Stat.Hp;
            set => Stat.Hp = Math.Clamp(value, 0, Stat.MaxHp);
        }

        public MoveDir Dir
        {
            get => PosInfo.MoveDir;
            set => PosInfo.MoveDir = value;
        }

        public CreatureState State
        {
            get => PosInfo.State;
            set => PosInfo.State = value;
        }
        
        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
        }

        public virtual void Update()
        {
            
        }
        
        public Vector2Int CellPos
        {
            get => new Vector2Int(PosInfo.PosX, PosInfo.PosY);
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }

        public Vector2Int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.MoveDir);
        }
        
        public Vector2Int GetFrontCellPos(MoveDir dir)
        {
            var cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;
            }

            return cellPos;
        }
        
        public static MoveDir GetDirFromVec(Vector2Int dir)
        {
            if (dir.x > 0)
            {
                return MoveDir.Right;
            }

            if (dir.x < 0)
            {
                return MoveDir.Left;
            }

            if (dir.y > 0)
            {
                return MoveDir.Up;
            }

            if (dir.y < 0)
            {
                return MoveDir.Down;
            }

            return MoveDir.Down;
        }

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            Stat.Hp = Math.Max(Stat.Hp - damage, 0);

            var changeHpPacket = new S_ChangeHp();
            changeHpPacket.ObjectId = Id;
            changeHpPacket.Hp = Stat.Hp;
            Room.Broadcast(changeHpPacket);
            
            if (Stat.Hp <= 0)
            {
                OnDead(attacker);
            }
        }
        
        public virtual void OnDead(GameObject attacker)
        {
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            var room = Room;
            room.LeaveGame(Id);

            Stat.Hp = Stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            PosInfo.MoveDir = MoveDir.Down;
            PosInfo.PosX = 0;
            PosInfo.PosY = 0;
            
            room.EnterGame(this);
        }
    }
}