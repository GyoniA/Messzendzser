using Messzendzser.Model.DB.Models;

namespace Messzendzser.WhiteBoard
{
    public class Whiteboard
    {
        private Chatroom room;
        private LinkedList<WhiteboardMessage> messages = new LinkedList<WhiteboardMessage>();
        private byte[] image;

        public Whiteboard(Chatroom room)
        {
            this.room = room;
        }

        public void AddMessage(WhiteboardMessage message)
        {
            messages.AddLast(message);
        }
        
        public byte[] GetData() {
            return image;
        }
    }
}
