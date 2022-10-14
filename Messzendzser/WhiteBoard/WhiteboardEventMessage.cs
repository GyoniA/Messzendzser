using Messzendzser.Model.DB.Models;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardEventMessage : WhiteboardMessage
    {

        private LinkedList<WhiteboardEvent> events = new LinkedList<WhiteboardEvent>();
        public Chatroom Chatroom { get; set; }
        
        public WhiteboardEventMessage(byte[] message, Chatroom chatroom) : base(message)
        {
            Chatroom = chatroom;
        }

        public void AddEvent(WhiteboardEvent e)
        {
            events.AddLast(e);
        }
        
        public LinkedList<WhiteboardEvent> GetEvents()
        {
            return events;
        }
    }
}
