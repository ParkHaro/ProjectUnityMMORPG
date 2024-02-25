using System.Collections.Generic;

namespace Server.Game
{
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();
        
        private object _lock = new object();
        private Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        private int _roomId = 1;

        public GameRoom Add(int mapId)
        {
            var gameRoom = new GameRoom();
            gameRoom.Init(mapId);
            
            lock (_lock)
            {
                gameRoom.RoomId = _roomId++;
                _rooms.Add(gameRoom.RoomId, gameRoom);
                _roomId++;
            }

            return gameRoom;
        }
        
        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }
        }
        
        public GameRoom Find(int roomId)
        {
            lock (_lock)
            {
                GameRoom room = null;
                if (_rooms.TryGetValue(roomId, out room))
                {
                    return room;
                }

                return null;
            }
        }
    }
}