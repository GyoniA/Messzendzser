using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Messzendzser.Model.DB.Models;
using Messzendzser.Model.Managers.Media;

namespace Messzendzser.Model.Managers.Message
{
    public class JsonImageMessage : JsonMessage
    {
        public string Token { get; set; }
        public string Format { get; set; }
        

        public JsonImageMessage()
        {
            Token = "";
            Format = "";
        }
        public JsonImageMessage(int userId, int chatroomId, DateTime time, string token, string format) : base(userId, chatroomId, time)
        {
            Token = token;
            Format = format;
        }

        public JsonImageMessage(ImageChatMessage message) : base(message.UserId, message.ChatroomId, message.SentTime)
        {
            Token = message.Token;
            Format = message.Format;
        }

        public override ISerializeableMessage Deserialize(byte[] jsonUTF8)
        {
            var utf8Reader = new Utf8JsonReader(jsonUTF8);
            JsonImageMessage message = JsonSerializer.Deserialize<JsonImageMessage>(ref utf8Reader)!;
            return message;
        }
    }
}
