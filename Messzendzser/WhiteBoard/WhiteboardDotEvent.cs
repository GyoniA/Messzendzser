using Messzendzser.Model.Managers.Message;
using SIPSorcery.Media;
using SkiaSharp;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardDotEvent : WhiteboardEvent
    {
        public Point Position { get; set; }
        public uint Color { get; set; }
        public WhiteboardDotEvent() : base(EventType.Dot)
        {
            Position = new Point(0, 0);
            Color = 0;
        }
        public WhiteboardDotEvent(Point position, uint color) : base(EventType.Dot)
        {
            Position = position;
            Color = color;
        }

        public override SKCanvas Draw(SKCanvas canvas)
        {
            canvas.DrawPoint(Position.X, Position.Y, new SKPaint() { Color = new SKColor(Color) });
            return canvas;
        }
    }
}
