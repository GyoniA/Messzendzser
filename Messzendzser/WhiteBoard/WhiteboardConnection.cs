namespace Messzendzser.WhiteBoard
{
    using Messzendzser.Model.DB.Models;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    public class WhiteboardConnection
    {
        public TcpClient Client { get; set; }
        public string Username { get; set; }
        public int RoomId { get; set; }
        public System.Timers.Timer IsAliveTimer { get; set; } = null;

        public WhiteboardConnection(string username, int room, TcpClient client)
        {
            Username = username;
            RoomId = room;
            Client = client;
        }
    }
}
