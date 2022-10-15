using Org.BouncyCastle.Crypto.Paddings;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public abstract class WhiteboardEvent
    {
        public byte[] Draw(byte[] image)
        {
            //TODO draw the event onto image
            return image;
        }

        public string Serialize() {
            //TODO check if correct
            string json = JsonSerializer.Serialize(this, this.GetType());
            return json;
        }

        public abstract WhiteboardEvent Deserialize(string data);
    }
}
