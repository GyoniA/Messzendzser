using Messzendzser.Model.DB.Models;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardEventMessage : WhiteboardMessage
    {

        public LinkedList<WhiteboardEvent> Events { get; set; }
        public Chatroom Chatroom { get; set; }
        
        public WhiteboardEventMessage(Chatroom chatroom) : base(MessageType.Event)
        {
            Events = new LinkedList<WhiteboardEvent>();
            Chatroom = chatroom;
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

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardEventMessage>(message);
        }
    }
}
