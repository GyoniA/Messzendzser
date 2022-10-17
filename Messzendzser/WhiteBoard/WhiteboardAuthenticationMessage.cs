using Messzendzser.Model.DB.Models;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardAuthenticationMessage : WhiteboardMessage
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int ChatroomId { get; set; }
        
        public WhiteboardAuthenticationMessage(string username, string password, int chatroom) : base(MessageType.Authentication)
        {
            Username = username;
            Password = password;
            ChatroomId = chatroom;
        }

        public WhiteboardAuthenticationMessage() : base(MessageType.Authentication)
        {
            Username = "";
            Password = "";
            ChatroomId = -1;
        }

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardAuthenticationMessage>(System.Text.Encoding.UTF8.GetString(message).TrimEnd('\0'));
        }
    }
}
