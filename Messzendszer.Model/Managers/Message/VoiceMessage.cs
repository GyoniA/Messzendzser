﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Messzendzser.Model.DB.Models;
using static System.Net.Mime.MediaTypeNames;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using Messzendzser.Model.Managers.Media;

namespace Messzendzser.Model.Managers.Message
{
    public class VoiceMessage : Message
    {
        public string Token { get; set; }
        public string Format { get; set; }
        public int Length { get; set; }

        public VoiceMessage()
        {
            Token = "";
            Format = "";
            Length = 0;
        }

        public VoiceMessage(int userId, int chatroomId, DateTime time, string token, string format, int length) : base(userId, chatroomId, time)
        {
            Token = token;
            Format = format;
            Length = length;
        }

        public VoiceMessage(VoiceChatMessage message) : base(message.UserId, message.ChatroomId, message.SentTime)
        {
            Token = message.Token;
            Format = message.Format;
            Length = message.Length;
        }
        /*
        public override ISerializeableMessage Deserialize(byte[] jsonUTF8)
        {
            var utf8Reader = new Utf8JsonReader(jsonUTF8);
            VoiceMessage message = JsonSerializer.Deserialize<VoiceMessage>(ref utf8Reader)!;
            return message;
        }*/
    }
}
