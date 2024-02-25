using System.Collections.Generic;

namespace Server.Game
{
    public class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();
        
        private object _lock = new object();
        private Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private int _playerId = 1; // TODO

        public Player Add()
        {
            var player = new Player();
            
            lock (_lock)
            {
                player.Info.PlayerId = _playerId;
                _players.Add(_playerId, player);
                _playerId++;
            }

            return player;
        }
        
        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return _players.Remove(playerId);
            }
        }
        
        public Player Find(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.TryGetValue(playerId, out player))
                {
                    return player;
                }

                return null;
            }
        }
    }
}