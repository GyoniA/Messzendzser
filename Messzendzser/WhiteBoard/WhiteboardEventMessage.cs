namespace Messzendzser.WhiteBoard
{
    public class WhiteboardEventMessage : WhiteboardMessage
    {        
        public LinkedList<WhiteboardEvent> Events { get; set; }
        
        public WhiteboardEventMessage(int chatroom) : base(MessageType.Event)
        {
            Events = new LinkedList<WhiteboardEvent>();
        }

        public WhiteboardEventMessage(int chatroom, LinkedList<WhiteboardEvent> events) : base(MessageType.Event)
        {
            Events = events;
        }

        public WhiteboardEventMessage() : base(MessageType.Event)
        {
            Events = new LinkedList<WhiteboardEvent>();
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
