using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardDeniedMessage : WhiteboardMessage
    {
        public WhiteboardDeniedMessage(byte[] message) : base(message)
        {
        }

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardDeniedMessage>(message);
        }
    }
}
