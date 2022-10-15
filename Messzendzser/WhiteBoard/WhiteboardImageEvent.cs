using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardImageEvent : WhiteboardEvent
    {
        Whiteboard board;

        public override WhiteboardEvent Deserialize(string data)
        {
            return JsonSerializer.Deserialize<WhiteboardImageEvent>(data);
        }
    }
}
