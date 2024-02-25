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
            if (Owner == null || Room == null)
            {
                return;
            }

            if (_nextMoveTick >= Environment.TickCount64)
            {
                return;
            }
            
            _nextMoveTick = Environment.TickCount64 + 50;

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