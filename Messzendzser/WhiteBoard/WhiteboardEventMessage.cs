using Google.Protobuf.WellKnownTypes;
using Messzendzser.Model.DB.Models;
using Messzendzser.Model.Managers.Message;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static LumiSoft.Net.MIME.MIME_MediaTypes;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardEventMessage : WhiteboardMessage
    {
        /*
        private class WhiteboardEventListConverter :
                JsonConverter<LinkedList<object>>
        {
            private readonly JsonConverter<WhiteboardEvent> valueConverter;
            private readonly EventType Type;
            public override LinkedList<object>? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var list = new LinkedList<WhiteboardEvent>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return (LinkedList<object>?)list.Cast<LinkedList<object>>();
                    }

                    // Get the value.
                    
                    JsonSerializer.Deserialize<WhiteboardDotEvent>(reader);
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

                    WhiteboardEvent wEvent = WhiteboardEvent.GetEventFromType(key);
                    wEvent = wEvent.Deserialize(reader);
                    reader.Read();
                    WhiteboardEvent value = _valueConverter.Read(ref reader, _valueType, options)!;
                    
                    list.AddLast(value);
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, LinkedList<object> value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                
                foreach (WhiteboardEvent we in value)
                {
                    string json = JsonSerializer.Serialize(we, we.GetType());
                    writer.WriteRawValue(json);
                }

                writer.WriteEndObject();
            }
        }



        [JsonConverter(typeof(WhiteboardEventListConverter))]*/
        public LinkedList<WhiteboardEvent> Events { get; set; }
        
        public WhiteboardEventMessage(int chatroom) : base(MessageType.Event)
        {
            Events = new LinkedList<WhiteboardEvent>();
        }

        public WhiteboardEventMessage(int chatroom, LinkedList<WhiteboardEvent> events) : base(MessageType.Event)
        {
            Events = events;
        }

        public WhiteboardEventMessage() : base(MessageType.Event)
        {
            Events = new LinkedList<WhiteboardEvent>();
        }

        public void AddEvent(WhiteboardEvent e)
        {
            Events.AddLast(e);
        }
        
        public LinkedList<WhiteboardEvent> GetEvents()
        {
            return Events;
        }

        public override WhiteboardMessage DeSerialize(byte[] message)
        {
            return JsonSerializer.Deserialize<WhiteboardEventMessage>(message);
        }
    }
}
