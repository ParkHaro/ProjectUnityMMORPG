using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Server.Data;
using Server.Game;
using ServerCore;
using Timer = System.Timers.Timer;

namespace Server
{
	class Program
	{
		private static Listener _listener = new Listener();
		private static List<Timer> _timers = new List<Timer>();

		private static void TickRoom(GameRoom room, int tick = 100)
		{
			var timer = new Timer();
			timer.Interval = tick;
			timer.Elapsed += ((s, e) => room.Update());
			timer.AutoReset = true;
			timer.Enabled = true;
			
			_timers.Add(timer);
		}
		
		private static void Main(string[] args)
		{
			ConfigManager.LoadConfig();
			DataManager.LoadData();
			
			var gameRoom = RoomManager.Instance.Add(1);
			TickRoom(gameRoom, 30);
			
			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			//FlushRoom();

			while (true)
			{
				Thread.Sleep(100);
			}
		}
	}
}
