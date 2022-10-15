using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.Message
{
    public abstract class Message : ISerializeableMessage
    {
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public DateTime Time { get; set; }

        public Message()
        {
            UserId = 0;
            ChatroomId = 0;
            Time = DateTime.Now;
        }

        public Message(int userId, int chatroomId, DateTime time)
        {
            UserId = userId;
            ChatroomId = chatroomId;
            Time = time;
        }
        
        public string Serialize() {
            string json = JsonSerializer.Serialize(this, this.GetType());
            return json;
        }
        /*
        public abstract ISerializeableMessage Deserialize(byte[] jsonUTF8);*/
    }
}
