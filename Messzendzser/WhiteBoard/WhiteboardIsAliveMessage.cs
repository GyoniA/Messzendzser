using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardIsAliveMessage : WhiteboardMessage
    {
        public WhiteboardIsAliveMessage(byte[] message) : base(message)
        {
        }

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardIsAliveMessage>(message);
        }
    }
}
