using Google.Protobuf.WellKnownTypes;
using Messzendzser.Model.DB.Models;
using Messzendzser.Model.Managers.Message;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static LumiSoft.Net.MIME.MIME_MediaTypes;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardEventMessage : WhiteboardMessage
    {        
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

        

        /*public override WhiteboardMessage DeSerialize(byte[] message)
        {
            
            return System.Text.Json.JsonSerializer.Deserialize<WhiteboardEventMessage>(System.Text.Encoding.UTF8.GetString(message).TrimEnd('\0'));
            
            WhiteboardEventMessage models = JsonConvert.DeserializeObject<WhiteboardEventMessage>(System.Text.Encoding.UTF8.GetString(message).TrimEnd('\0')), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }*/
        /*
        public override WhiteboardMessage DeSerialize(byte[] message)//TODO move this to descendants
        {
            using (var stream = new MemoryStream(message))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return Newtonsoft.Json.JsonSerializer.Create().Deserialize(reader, typeof(WhiteboardEventMessage)) as WhiteboardEventMessage;
            }
        }*/
    }
}
