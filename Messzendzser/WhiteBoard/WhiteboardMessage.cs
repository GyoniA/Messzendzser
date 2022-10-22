using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Text;
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

        public byte[] Serialize()
        {
            var stringData = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            return Encoding.UTF8.GetBytes(stringData);
        }
        
        public WhiteboardMessage DeSerialize(byte[] message)//TODO move this to descendants
        {
            WhiteboardMessage wbmessage = JsonConvert.DeserializeObject<WhiteboardMessage>(Encoding.ASCII.GetString(message), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return wbmessage;
        }

        public static MessageType GetMessageType(byte[] message)
        {
            try
            {
                string json = System.Text.Encoding.UTF8.GetString(message).TrimEnd('\0');
                using var jDoc = JsonDocument.Parse(json);
                var myClass = jDoc.RootElement.GetProperty("Type").Deserialize<MessageType>();


                return myClass;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n\nError in GetMessageType: {e.Message}\n\n");
                throw;
            }
        }
    }
}
