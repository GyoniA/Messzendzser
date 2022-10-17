using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardIsAliveMessage : WhiteboardMessage
    {
        public WhiteboardIsAliveMessage() : base(MessageType.IsAlive)
        {
        }

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardIsAliveMessage>(System.Text.Encoding.UTF8.GetString(message).TrimEnd('\0'));
        }
    }
}
