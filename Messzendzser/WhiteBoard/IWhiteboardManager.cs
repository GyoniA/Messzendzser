using Messzendzser.Model.DB.Models;

namespace Messzendzser.WhiteBoard
{
    public interface IWhiteboardManager
    {
        public void UpdateWhiteboard(byte[] message);
        public byte[] GetWhiteboardData();
    }
}
