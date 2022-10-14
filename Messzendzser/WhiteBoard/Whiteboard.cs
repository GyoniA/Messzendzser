using Messzendzser.Model.DB.Models;
using System.Collections.Immutable;

namespace Messzendzser.WhiteBoard
{
    public class Whiteboard
    {
        private Chatroom room;
        private ImmutableList<WhiteboardConnection> connections = ImmutableList<WhiteboardConnection>.Empty;
        private ImmutableList<WhiteboardEvent> events = ImmutableList<WhiteboardEvent>.Empty;
        private byte[] image = new byte[0];

        public Whiteboard(Chatroom room)
        {
            this.room = room;
        }

        public void AddConnection(WhiteboardConnection connection)
        {
            connections = connections.Add(connection);
        }

        private void Draw(WhiteboardEvent e)
        {
            //draw the event onto image
        }
        
        public void AddEvents(LinkedList<WhiteboardEvent> newEvents)
        {
            //events.AddRange(newEvents);
            foreach (var e in newEvents)
            {
                Draw(e);
                events.Add(e);
            }
            foreach (var c in connections)
            {
                //TODO send changes to clients
                c.Client.GetStream();
            }
        }

        public byte[] GetData()
        {
            return image;
        }
    }
}
