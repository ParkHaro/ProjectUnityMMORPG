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
        
        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
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

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            
        }
    }
}