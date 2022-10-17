using Messzendzser.Model.Managers.Media;
using Org.BouncyCastle.Utilities;
using SkiaSharp;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardImageEvent : WhiteboardEvent
    {
        public Whiteboard Board { get; set; }

        public override WhiteboardEvent Deserialize(string data)
        {
            return JsonSerializer.Deserialize<WhiteboardImageEvent>(data);
        }

        private SKBitmap HighlightWord(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);

            SKBitmap bitmap = SKBitmap.Decode(ms);
            var canvas = new SKCanvas(bitmap);
            

            return bitmap;
        }

        public override SKCanvas Draw(SKCanvas canvas)
        {
            MediaManager mm = new MediaManager();
            byte[] data = mm.LoadWhiteboard(Board.RoomId);

            var ms = new MemoryStream(data);

            SKBitmap bitmap = SKBitmap.Decode(ms);
            var image = new SKCanvas(bitmap);

            return image;
        }
    }
}
