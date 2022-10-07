using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.Message
{
    public abstract class JsonMessage : ISerializeableMessage
    {
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public DateTime Time { get; set; }

        public JsonMessage()
        {
            UserId = 0;
            ChatroomId = 0;
            Time = DateTime.Now;
        }

        public JsonMessage(int userId, int chatroomId, DateTime time)
        {
            UserId = userId;
            ChatroomId = chatroomId;
            Time = time;
        }

        public byte[] Serialize() {
            byte[] jsonUTF8 = JsonSerializer.SerializeToUtf8Bytes(this, this.GetType());
            return jsonUTF8;
        }

        public abstract ISerializeableMessage Deserialize(byte[] jsonUTF8);
    }
}
