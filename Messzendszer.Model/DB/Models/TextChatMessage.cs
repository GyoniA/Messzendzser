using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Messzendzser.Model.DB.Models
{
    [Table("text_chat_message")]
    [Index("ChatroomId", Name = "chatroom_id_idx")]
    [Index("UserId", Name = "user_id_idx")]
    [MySqlCharSet("latin1")]
    [MySqlCollation("latin1_swedish_ci")]
    public partial class TextChatMessage
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
        [Column("message")]
        [StringLength(500)]
        public string Message { get; set; } = null!;

        [ForeignKey("ChatroomId")]
        [InverseProperty("TextChatMessages")]
        public virtual Chatroom Chatroom { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("TextChatMessages")]
        public virtual User User { get; set; } = null!;
    }
}
