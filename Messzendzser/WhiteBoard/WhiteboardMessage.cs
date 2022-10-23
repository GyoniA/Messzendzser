using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System.Text;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardMessageConverter : CustomCreationConverter<WhiteboardMessage>
    {
        private MessageType _currentObjectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jobj = JObject.ReadFrom(reader);
            _currentObjectType = jobj["Type"].ToObject<MessageType>();
            return base.ReadJson(jobj.CreateReader(), objectType, existingValue, serializer);
        }

        public override WhiteboardMessage Create(Type objectType)
        {
            switch (_currentObjectType)
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
                    throw new NotImplementedException();
            }
        }
    }
    public enum MessageType
        {
            Authentication = 0,
            Denied = 1,
            OK = 2,
            IsAlive = 3,
            Event = 4
        }
    [Newtonsoft.Json.JsonConverter(typeof(WhiteboardMessageConverter))]
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
        
        public static WhiteboardMessage DeSerialize(byte[] message)//TODO move this to descendants
        {
            WhiteboardMessage wbmessage = JsonConvert.DeserializeObject<WhiteboardMessage>(Encoding.ASCII.GetString(message), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return wbmessage;
        }
    }
}
