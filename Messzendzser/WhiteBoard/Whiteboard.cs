using Messzendzser.Model.DB.Models;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using SkiaSharp;
using SIPSorcery.Media;
using Messzendzser.Model.Managers.Media;

namespace Messzendzser.WhiteBoard
{
    public class Whiteboard
    {
        public Chatroom Room { get; set; }
        private ConcurrentDictionary<String, WhiteboardConnection> connections = new ConcurrentDictionary<string, WhiteboardConnection>();
        private ConcurrentQueue<WhiteboardEvent> events = new ConcurrentQueue<WhiteboardEvent>();
        private SKImageInfo imageInfo;
        private SKSurface surface;
        public SKCanvas Canvas { get; private set; }

        public Whiteboard(Chatroom room)
        {
            this.Room = room;
            imageInfo = new SKImageInfo(width: 1920,
                                        height: 1080,
                                        colorType: SKColorType.Rgba8888,
                                        alphaType: SKAlphaType.Premul);

            surface = SKSurface.Create(imageInfo);

            Canvas = surface.Canvas;

            Canvas.Clear(SKColor.Parse("#FFFFFF"));
        }

        public void AddConnection(WhiteboardConnection connection)
        {
            if (!connections.TryAdd(connection.Username, connection))
            {
                connections[connection.Username] = connection;
            }
        }

        public void RemoveConnection(WhiteboardConnection connection)
        {
            connections.TryRemove(connection.Username, out _);
            if (connections.Count == 0)
            {
                new MediaManager().StoreWhiteboard(GetData(), Room.Id);
            }
        }

        private void Draw(WhiteboardEvent e)
        {
            Canvas = e.Draw(Canvas);
        }

        public void AddEvents(LinkedList<WhiteboardEvent> newEvents)
        {
            foreach (var e in newEvents)
            {
                Draw(e);
                events.Enqueue(e);
            }
            var wm = new WhiteboardManager();
            foreach (var c in connections)
            {
                c.Value.Client.GetStream();
                //TODO put changes into the eventMessage
                var data = new WhiteboardEventMessage(new byte[0], Room).Serialize();
                wm.SendMessageWithCheck(c.Value.Client, c.Value.Client.GetStream(), c.Value, c.Value.IsAliveTimer, data);
            }
        }

        public byte[] GetData()
        {
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            {
                // save the data to a stream
                return data.ToArray();
            }
        }
    }
}
