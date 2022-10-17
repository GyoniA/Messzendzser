using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardDeniedMessage : WhiteboardMessage
    {
        public WhiteboardDeniedMessage() : base(MessageType.Denied)
        {
        }

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardDeniedMessage>(System.Text.Encoding.UTF8.GetString(message).TrimEnd('\0'));
        }
    }
}
