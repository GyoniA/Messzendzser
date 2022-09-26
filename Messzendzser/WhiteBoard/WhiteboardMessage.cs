namespace Messzendzser.WhiteBoard
{
    public class WhiteboardMessage
    {
        private LinkedList<WhiteboardEvent> changes = new LinkedList<WhiteboardEvent>();

        public WhiteboardMessage(byte[] message)
        {
            this.changes.AddLast(new WhiteboardEvent(message));
        }
    }
}
