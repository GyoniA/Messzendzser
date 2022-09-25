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
        private TcpClient client;
        private string username;
        private Chatroom room;
    }
}
