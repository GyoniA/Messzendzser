using Org.BouncyCastle.Crypto.Paddings;
using System.Text.Json;
using SkiaSharp;

namespace Messzendzser.WhiteBoard
{
    public abstract class WhiteboardEvent
    {
        public abstract SKCanvas Draw(SKCanvas canvas);

        public string Serialize() {
            //TODO check if correct
            string json = JsonSerializer.Serialize(this, this.GetType());
            return json;
        }

        public abstract WhiteboardEvent Deserialize(string data);
    }
}
