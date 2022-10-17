using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public enum MessageType
        {
            Authentication = 0,
            Denied = 1,
            OK = 2,
            IsAlive = 3,
            Event = 4
        }
    public abstract class WhiteboardMessage
    {
        public MessageType Type { get; set; }

        public WhiteboardMessage(MessageType type)
        {
            Type = type;
        }


        public static WhiteboardMessage GetMessageFromType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Authentication:
                    return new WhiteboardAuthenticationMessage();
                case MessageType.Denied:
                    return new WhiteboardDeniedMessage();
                case MessageType.OK:
                    return new WhiteboardOKMessage();
                case MessageType.IsAlive:
                    return new WhiteboardIsAliveMessage();
                case MessageType.Event:
                    return new WhiteboardEventMessage();
                default:
                    return null;
            }
        }

        public byte[] Serialize() {
            byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(this, this.GetType());
            return jsonUtf8Bytes;
        }

        public abstract WhiteboardMessage DeSerialize(byte[] message);

        public static MessageType GetMessageType(byte[] message)
        {
            System.Text.Encoding.UTF8.GetString(message);
            dynamic tmp = JsonSerializer.Deserialize<object>(message);
            return (MessageType)tmp.Type;
        }
    }
}
