using Messzendzser.Model.DB.Models;
using Messzendzser.Model.Managers.Media;
using SkiaSharp;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Messzendzser.WhiteBoard
{
    public class Whiteboard
    {
        public int RoomId { get; set; }
        private ConcurrentDictionary<String, WhiteboardConnection> connections = new ConcurrentDictionary<string, WhiteboardConnection>();
        private ConcurrentQueue<WhiteboardEvent> events = new ConcurrentQueue<WhiteboardEvent>();
        private SKImageInfo imageInfo;
        private SKSurface surface;
        public SKCanvas Canvas { get; private set; }

        public Whiteboard(int room)
        {
            this.RoomId = room;
            imageInfo = new SKImageInfo(width: 1920,
                                        height: 1080,
                                        colorType: SKColorType.Rgba8888,
                                        alphaType: SKAlphaType.Premul);

            surface = SKSurface.Create(imageInfo);

            Canvas = surface.Canvas;

            Canvas.Clear(SKColor.Parse("#FFFFFF"));
        }

        public void SaveDataToFile()
        {
            new MediaManager().StoreWhiteboard(GetData(), RoomId);
            events.Clear();/*
            LinkedList<WhiteboardEvent> imageEv = new LinkedList<WhiteboardEvent>();
            imageEv.AddLast(new WhiteboardImageEvent(RoomId));
            AddEvents(imageEv, );*/
        }
        
        public void AddConnection(WhiteboardConnection connection)
        {
            if (connections.Count > 0)
            {
                SaveDataToFile();
            }
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
                SaveDataToFile();
            }
        }

        private void Draw(WhiteboardEvent e)
        {
            Canvas = e.Draw(Canvas);
        }

        public void LoadImageFromFile()
        {
            WhiteboardImageEvent wbim = new WhiteboardImageEvent(RoomId);
            Draw(wbim);
        }

        public async Task AddEvents(LinkedList<WhiteboardEvent> newEvents,WhiteboardManager whiteboardManager, WhiteboardConnection sender)
        {
            foreach (var e in newEvents)
            {
                Draw(e);
                events.Enqueue(e);
            }
            WhiteboardEventMessage wem = new WhiteboardEventMessage(RoomId);
            wem.Events = newEvents;
            foreach (var c in connections)
            {
                if (c.Value != sender)
                {
                    var data = wem.Serialize();
                    await whiteboardManager.SendMessageWithCheck(c.Value.Client, c.Value, c.Value.IsAliveTimer, data);
                }
            }
        }

        public byte[] GetData()
        {
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            {
                // return the image as a byte array
                return data.ToArray();
            }
        }
    }
}