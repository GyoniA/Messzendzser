using Messzendzser.Model.DB.Models;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardEventMessage : WhiteboardMessage
    {

        public LinkedList<WhiteboardEvent> Events { get; set; }
        public int ChatroomId { get; set; }
        
        public WhiteboardEventMessage(int chatroom) : base(MessageType.Event)
        {
            Events = new LinkedList<WhiteboardEvent>();
            ChatroomId = chatroom;
        }

        public WhiteboardEventMessage(int chatroom, LinkedList<WhiteboardEvent> events) : base(MessageType.Event)
        {
            Events = events;
            ChatroomId = chatroom;
        }

        public WhiteboardEventMessage() : base(MessageType.Event)
        {
            Events = new LinkedList<WhiteboardEvent>();
            ChatroomId = -1;
        }

        public void AddEvent(WhiteboardEvent e)
        {
            Events.AddLast(e);
        }
        
        public LinkedList<WhiteboardEvent> GetEvents()
        {
            return Events;
        }

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardEventMessage>(message);
        }
    }
}
