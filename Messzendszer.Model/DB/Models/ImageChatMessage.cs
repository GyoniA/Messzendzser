using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Messzendzser.Model.DB.Models
{
    [Table("image_chat_message")]
    [Index("ChatroomId", Name = "image_chat_message_chatroom_id_idx")]
    [Index("UserId", Name = "image_chat_message_user_id_idx")]
    [MySqlCharSet("latin1")]
    [MySqlCollation("latin1_swedish_ci")]
    public partial class ImageChatMessage
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

        [ForeignKey("ChatroomId")]
        [InverseProperty("ImageChatMessages")]
        public virtual Chatroom Chatroom { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("ImageChatMessages")]
        public virtual User User { get; set; } = null!;
    }
}
