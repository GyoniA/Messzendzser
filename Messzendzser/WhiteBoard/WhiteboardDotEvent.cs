using Messzendzser.Model.Managers.Message;
using SIPSorcery.Media;
using SkiaSharp;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardDotEvent : WhiteboardEvent
    {
        public SKPoint Position { get; set; }
        public uint Color { get; set; }

        public WhiteboardDotEvent(SKPoint position, uint color)
        {
            Position = position;
            Color = color;
        }

        public override WhiteboardEvent Deserialize(string data)
        {
            return JsonSerializer.Deserialize<WhiteboardDotEvent>(data);
        }

        public override SKCanvas Draw(SKCanvas canvas)
        {
            canvas.DrawPoint(Position, new SKPaint() { Color = new SKColor(Color) });
            return canvas;
        }
    }
}
