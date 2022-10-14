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
    public class WhiteboardMessage
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
            throw new NotImplementedException();
        }

        public WhiteboardMessage DeSerialize(byte[] message)
        {
            //return new WhiteboardMessage(MessageType.Authentication);
            throw new NotImplementedException();
        }
    }
}
