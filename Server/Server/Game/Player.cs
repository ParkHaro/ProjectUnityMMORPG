using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo() { PosInfo = new PositionInfo() };
        public GameRoom Room { get; set; }
        public ClientSession Session { get; set; }
    }
}