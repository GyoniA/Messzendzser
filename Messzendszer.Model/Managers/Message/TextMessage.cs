using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Messzendzser.Model.DB.Models;

namespace Messzendzser.Model.Managers.Message
{
    public class TextMessage : Message
    {
        public string Text { get; set; }

        public TextMessage()
        {
            Text = "";
        }
        public TextMessage(int userId, int chatroomId, DateTime time, string text) : base(userId, chatroomId, time)
        {
            Text = text;
        }

        public TextMessage(TextChatMessage message) : base(message.UserId, message.ChatroomId, message.SentTime)
        {
            Text = message.Message;
        }
        /*
        public override ISerializeableMessage Deserialize(byte[] jsonUTF8)
        {
            var utf8Reader = new Utf8JsonReader(jsonUTF8);
            TextMessage message = JsonSerializer.Deserialize<TextMessage>(ref utf8Reader)!;
            return message;
        }*/
    }
}
