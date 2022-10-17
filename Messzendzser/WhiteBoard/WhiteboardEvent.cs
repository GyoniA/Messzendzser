using Org.BouncyCastle.Crypto.Paddings;
using System.Text.Json;
using SkiaSharp;

namespace Messzendzser.WhiteBoard
{
    public enum EventType
    {
        Image = 0,
        Dot = 1,
        Line = 2
    }
    public abstract class WhiteboardEvent
    {
        public EventType Type { get; set; }
        public abstract SKCanvas Draw(SKCanvas canvas);

        public WhiteboardEvent(EventType type)
        {
            Type = type;
        }


        public static WhiteboardEvent GetEventFromType(EventType type)
        {
            switch (type)
            {
                case EventType.Image:
                    return new WhiteboardImageEvent();
                case EventType.Dot:
                    return new WhiteboardDotEvent();
                case EventType.Line:
                    return new WhiteboardLineEvent();
                default:
                    return null;
            }
        }

        public string Serialize() {
            //TODO check if correct
            string json = JsonSerializer.Serialize(this, this.GetType());
            return json;
        }

        public abstract WhiteboardEvent Deserialize(string data);
    }
}
