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
        private Chatroom room;

        public WhiteboardConnection(string username, Chatroom room, TcpClient client)
        {
            this.Username = username;
            this.room = room;
            this.Client = client;
        }
    }
}
