using Messzendzser.Model.DB.Models;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardAuthenticationMessage : WhiteboardMessage
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Chatroom Chatroom { get; set; }

        public WhiteboardAuthenticationMessage(byte[] message, string username, string password, Chatroom chatroom) : base(message)
        {
            Username = username;
            Password = password;
            Chatroom = chatroom;
        }
    }
}
