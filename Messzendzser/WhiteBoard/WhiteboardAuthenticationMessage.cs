using Messzendzser.Model.DB.Models;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardAuthenticationMessage : WhiteboardMessage
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Chatroom Chatroom { get; set; }

        public WhiteboardAuthenticationMessage(string username, string password, Chatroom chatroom) : base(MessageType.Authentication)
        {
            Username = username;
            Password = password;
            Chatroom = chatroom;
        }

        public WhiteboardAuthenticationMessage() : base(MessageType.Authentication)
        {
            Username = "";
            Password = "";
            Chatroom = null;
        }

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardAuthenticationMessage>(message);
        }
    }
}
