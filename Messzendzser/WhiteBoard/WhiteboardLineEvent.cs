using SkiaSharp;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardLineEvent : WhiteboardEvent
    {
        public SKPoint Start { get; set; }
        public SKPoint End { get; set; }
        public uint Color { get; set; }

        public WhiteboardLineEvent(SKPoint start, SKPoint end, uint color)
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
            canvas.DrawLine(Start, End, new SKPaint() { Color = new SKColor(Color) });
            return canvas;
        }

    }
}
