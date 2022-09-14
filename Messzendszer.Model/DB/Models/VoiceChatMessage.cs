using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Messzendzser.Model.DB.Models
{
    [Table("voice_chat_message")]
    [Index("ChatroomId", Name = "voice_chat_message_chatroom_id_idx")]
    [Index("UserId", Name = "voice_chat_message_user_id_idx")]
    [MySqlCharSet("latin1")]
    [MySqlCollation("latin1_swedish_ci")]
    public partial class VoiceChatMessage
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("user_id", TypeName = "int(11)")]
        public int UserId { get; set; }
        [Column("sent_time", TypeName = "datetime")]
        public DateTime SentTime { get; set; }
        [Column("chatroom_id", TypeName = "int(11)")]
        public int ChatroomId { get; set; }
        [Column("token")]
        [StringLength(60)]
        public string Token { get; set; } = null!;
        [Column("length", TypeName = "int(11)")]
        public int Length { get; set; }

        [ForeignKey("ChatroomId")]
        [InverseProperty("VoiceChatMessages")]
        public virtual Chatroom Chatroom { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("VoiceChatMessages")]
        public virtual User User { get; set; } = null!;
    }
}
