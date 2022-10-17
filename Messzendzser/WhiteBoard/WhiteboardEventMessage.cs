using Messzendzser.Model.DB.Models;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardEventMessage : WhiteboardMessage
    {

        public LinkedList<WhiteboardEvent> Events { get; set; }
        public Chatroom Chatroom { get; set; }
        
        public WhiteboardEventMessage(byte[] message, Chatroom chatroom) : base(message)
        {
            Events = new LinkedList<WhiteboardEvent>();
            Chatroom = chatroom;
        }

        public void AddEvent(WhiteboardEvent e)
        {
            Events.AddLast(e);
        }
        
        public LinkedList<WhiteboardEvent> GetEvents()
        {
            return Events;
        }
    }
}
