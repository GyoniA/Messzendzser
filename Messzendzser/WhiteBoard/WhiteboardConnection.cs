namespace Messzendzser.WhiteBoard
{
    using Messzendzser.Model.DB.Models;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    public class WhiteboardConnection
    {
        public WebSocket Client { get; set; }
        public string Username { get; set; }
        public Chatroom Room { get; set; }
        public System.Timers.Timer IsAliveTimer { get; set; } = null;

        public WhiteboardConnection(string username, Chatroom room, WebSocket client)
        {
            Username = username;
            Room = room;
            Client = client;
        }
    }
}
