using Messzendzser.Model.Managers.Media;
using Org.BouncyCastle.Utilities;
using SkiaSharp;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardImageEvent : WhiteboardEvent
    {
        public int RoomId { get; set; }

        public string Image { get; set; }

        public WhiteboardImageEvent() : base(EventType.Image)
        {
            RoomId = -1;
            Image = Convert.ToBase64String(Array.Empty<byte>());
        }
        public WhiteboardImageEvent(int id) : base(EventType.Image)
        {
            RoomId = id;
            MediaManager mm = new MediaManager();
            Image = Convert.ToBase64String(mm.LoadWhiteboard(RoomId));
        }

        public override SKCanvas Draw(SKCanvas canvas)
        {
            MediaManager mm = new MediaManager();
            byte[] data = mm.LoadWhiteboard(RoomId);
            
            var ms = new MemoryStream(data);

            SKBitmap bitmap = SKBitmap.Decode(ms);
            var image = new SKCanvas(bitmap);

            return image;
        }
    }
}
