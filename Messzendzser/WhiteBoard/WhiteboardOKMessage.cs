using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardOKMessage : WhiteboardMessage
    {
        public WhiteboardOKMessage(byte[] message) : base(message)
        {
        }
        public override WhiteboardMessage DeSerialize(byte[] message) {
            return JsonSerializer.Deserialize<WhiteboardOKMessage>(message);
        }
    }
}
