using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardOKMessage : WhiteboardMessage
    {
        public WhiteboardOKMessage() : base(MessageType.OK)
        {
        }
        public override WhiteboardMessage DeSerialize(byte[] message) {
            return JsonSerializer.Deserialize<WhiteboardOKMessage>(System.Text.Encoding.UTF8.GetString(message).TrimEnd('\0'));
        }
    }
}
