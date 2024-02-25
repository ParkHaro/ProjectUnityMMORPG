using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();

        private object _lock = new object();
        private Dictionary<int, Player> _players = new Dictionary<int, Player>();

        // [UNUSED(1)][TYPE(7)][ID(24)]
        // [........|........|........|........]
        private int _counter = 0; // TODO

        public T Add<T>() where T : GameObject, new()
        {
            T gameObject = new T();

            lock (_lock)
            {
                gameObject.Id = GenerateId(gameObject.ObjectType);

                if (gameObject.ObjectType == GameObjectType.Player)
                {
                    _players.Add(gameObject.Id, gameObject as Player);
                }
            }

            return gameObject;
        }

        private int GenerateId(GameObjectType type)
        {
            lock (_lock)
            {
                return ((int)type << 24) | (_counter++);
            }
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            var type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }

        public bool Remove(int playerId)
        {
            var objectType = GetObjectTypeById(playerId);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    return _players.Remove(playerId);
                }
            }

            return false;
        }

        public Player Find(int playerId)
        {
            var objectType = GetObjectTypeById(playerId);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.TryGetValue(playerId, out player))
                    {
                        return player;
                    }
                }

                return null;
            }
        }
    }
}