using Messzendzser.Model.DB.Models;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Messzendzser.WhiteBoard
{
    public class Whiteboard
    {
        private Chatroom room;
        //private ImmutableList<WhiteboardConnection> connections = ImmutableList<WhiteboardConnection>.Empty;
        private ConcurrentDictionary<String, WhiteboardConnection> connections = new ConcurrentDictionary<string, WhiteboardConnection>();
        //private ImmutableList<WhiteboardEvent> events = ImmutableList<WhiteboardEvent>.Empty;
        private ConcurrentQueue<WhiteboardEvent> events = new ConcurrentQueue<WhiteboardEvent>();
        private byte[] image = new byte[0];

        public Whiteboard(Chatroom room)
        {
            this.room = room;
        }
        
        public void AddConnection(WhiteboardConnection connection)
        {
            if (!connections.TryAdd(connection.Username, connection))
            {
                connections[connection.Username] = connection;
            }
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
                events.Enqueue(e);
            }
            foreach (var c in connections)
            {
                //TODO send changes to clients
                c.Value.Client.GetStream();
            }
        }

        public byte[] GetData()
        {
            return image;
        }
    }
}
