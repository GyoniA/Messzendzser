using Messzendzser.Model.Managers.Message;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardDotEvent : WhiteboardEvent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Color { get; set; }

        public override WhiteboardEvent Deserialize(string data)
        {
            return JsonSerializer.Deserialize<WhiteboardDotEvent>(data);
        }
    }
}
