using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Messzendzser.Model.DB.Models;

namespace Messzendzser.Model.Managers.Message
{
    public class JsonVoiceMessage : JsonMessage
    {
        public string Token { get; set; }
        public string Format { get; set; }
        public int Length { get; set; }
        
        public JsonVoiceMessage(int userId, int chatroomId, DateTime time, string token, string format, int length) : base(userId, chatroomId, time)
        {
            Token = token;
            Format = format;
            Length = length;
        }
        
        public override ISerializeableMessage Deserialize(byte[] jsonUTF8)
        {
            var utf8Reader = new Utf8JsonReader(jsonUTF8);
            JsonVoiceMessage message = JsonSerializer.Deserialize<JsonVoiceMessage>(ref utf8Reader)!;
            return message;
        }
    }
}
