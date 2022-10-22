using SkiaSharp;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardLineEvent : WhiteboardEvent
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public uint Color { get; set; }

        public WhiteboardLineEvent() : base(EventType.Line)
        {
            Start = new Point(0, 0);
            End = new Point(0, 0);
            Color = 0;
        }

        public WhiteboardLineEvent(Point start, Point end, uint color) : base(EventType.Line)
        {
            Start = start;
            End = end;
            Color = color;
        }

        public override WhiteboardEvent Deserialize(string data)
        {
            return JsonSerializer.Deserialize<WhiteboardLineEvent>(data);
        }

        public override SKCanvas Draw(SKCanvas canvas)
        {
            canvas.DrawLine(Start.X, Start.Y, End.X, End.Y, new SKPaint() { Color = new SKColor(Color) });
            return canvas;
        }

    }
}
