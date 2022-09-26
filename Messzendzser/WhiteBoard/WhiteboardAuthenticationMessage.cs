using Messzendzser.Model.DB.Models;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardAuthenticationMessage
    {
        private string username;
        private string password;
        private Chatroom chatroom;

        public WhiteboardAuthenticationMessage(string username, string password, Chatroom chatroom)
        {
            this.username = username;
            this.password = password;
            this.chatroom = chatroom;
        }
    }
}
