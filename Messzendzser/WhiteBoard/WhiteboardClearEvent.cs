using Messzendzser.Model.Managers.Media;
using SkiaSharp;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardClearEvent : WhiteboardEvent
    {
        public WhiteboardClearEvent() : base(EventType.Clear)
        {
        }

        public override SKCanvas Draw(SKCanvas canvas)
        {
            canvas.Clear(SkiaSharp.SKColors.White);

            return canvas;
        }
    }
}
