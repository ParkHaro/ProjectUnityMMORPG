using System;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        public GameObject Owner { get; set; }

        private long _nextMoveTick = 0;
        
        public override void Update()
        {
            if (Data == null || Data.projectile == null || Owner == null || Room == null)
            {
                return;
            }

            if (_nextMoveTick >= Environment.TickCount64)
            {
                return;
            }

            var tick = (long)(1000 / Data.projectile.speed);
            _nextMoveTick = Environment.TickCount64 + tick;

            var destPos = GetFrontCellPos();
            if (Room.Map.CanGo(destPos))
            {
                CellPos = destPos;
                
                var movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.Broadcast(movePacket);

                Console.WriteLine("Move Arrow");
            }
            else
            {
                var target = Room.Map.Find(destPos);
                if (target != null)
                {
                    // TODO 피격
                }
                
                Room.LeaveGame(Id);
            }
        }
    }
}