using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Messzendzser.Model.DB.Models
{
    [Table("chatroom")]
    [MySqlCharSet("latin1")]
    [MySqlCollation("latin1_swedish_ci")]
    public partial class Chatroom
    {
        public Chatroom()
        {
            ImageChatMessages = new HashSet<ImageChatMessage>();
            TextChatMessages = new HashSet<TextChatMessage>();
            VoiceChatMessages = new HashSet<VoiceChatMessage>();
            Users = new HashSet<User>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(45)]
        public string? Name { get; set; }
        [Column("icon")]
        [StringLength(45)]
        public string? Icon { get; set; }

        [InverseProperty("Chatroom")]
        public virtual ICollection<ImageChatMessage> ImageChatMessages { get; set; }
        [InverseProperty("Chatroom")]
        public virtual ICollection<TextChatMessage> TextChatMessages { get; set; }
        [InverseProperty("Chatroom")]
        public virtual ICollection<VoiceChatMessage> VoiceChatMessages { get; set; }

        [ForeignKey("ChatroomId")]
        [InverseProperty("Chatrooms")]
        public virtual ICollection<User> Users { get; set; }
    }
}
