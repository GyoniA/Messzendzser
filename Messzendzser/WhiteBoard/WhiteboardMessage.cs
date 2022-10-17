using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public enum MessageType
        {
            Authentication,
            Denied,
            OK,
            IsAlive,
            Event
        }
    public abstract class WhiteboardMessage
    {
        public MessageType MessageType { get; set; }

        public WhiteboardMessage(byte[] message)
        {
            WhiteboardMessage wm = DeSerialize(message);
            MessageType = wm.MessageType;
        }

        public WhiteboardMessage(MessageType messageType)
        {
            MessageType = messageType;
        }

        public byte[] Serialize() {
            byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(this, this.GetType());
            return jsonUtf8Bytes;
        }

        public abstract WhiteboardMessage DeSerialize(byte[] message);
    }
}
