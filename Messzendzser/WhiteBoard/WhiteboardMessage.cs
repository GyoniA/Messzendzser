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
        {/*
            foreach (var message in transportMessageList)
            {
                MemoryStream ms = new MemoryStream();
                using (BsonDataWriter writer = new BsonDataWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, message);
                }

                var bsonByteArray = ms.ToArray();
                Assert.True(bsonByteArray.Length != 0);
                bsonList.Add(bsonByteArray);
            }

            var deserializdTransmortMessageList = new List<TransportMessage>();
            foreach (var byteArray in bsonList)
            {
                TransportMessage message;
                MemoryStream ms = new MemoryStream(byteArray);
                using (BsonDataReader reader = new BsonDataReader(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    message = serializer.Deserialize<TransportMessage>(reader);
                }
                Assert.True(message.Transportdata.Length != 0);
                deserializdTransmortMessageList.Add(message);


            var serializer = new Newtonsoft.Json.JsonSerializer();

            MemoryStream ms = new MemoryStream();
            using (var sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
            }*/
            var stringData = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            return Encoding.UTF8.GetBytes(stringData);
        }
        

        
        /*
        public byte[] Serialize() {
            /*
            string jsonTypeNameAll = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });*//*
            //TODO change to newton serializer
            byte[] jsonUtf8Bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(this, this.GetType());
            return jsonUtf8Bytes;
        }*/
        
        //public abstract WhiteboardMessage DeSerialize(byte[] message);
        
        public WhiteboardMessage DeSerialize(byte[] message)//TODO move this to descendants
        {/*
            using (var stream = new MemoryStream(message))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return Newtonsoft.Json.JsonSerializer.Create().Deserialize(reader, typeof(WhiteboardMessage)) as WhiteboardMessage;
            }*/

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
