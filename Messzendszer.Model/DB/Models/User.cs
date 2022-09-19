using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Messzendzser.Model.DB.Models
{
    [Table("user")]
    [MySqlCharSet("latin1")]
    [MySqlCollation("latin1_swedish_ci")]
    public partial class User
    {
        public User()
        {
            ImageChatMessages = new HashSet<ImageChatMessage>();
            TextChatMessages = new HashSet<TextChatMessage>();
            VoiceChatMessages = new HashSet<VoiceChatMessage>();
            Chatrooms = new HashSet<Chatroom>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("email")]
        [StringLength(60)]
        public string Email { get; set; } = null!;
        [Column("username")]
        [StringLength(60)]
        public string Username { get; set; } = null!;
        [Column("password")]
        [StringLength(90)]
        public string Password { get; set; } = null!;

        [InverseProperty("User")]
        public virtual VoipCredential? VoipCredential { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<ImageChatMessage> ImageChatMessages { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<TextChatMessage> TextChatMessages { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<VoiceChatMessage> VoiceChatMessages { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Users")]
        public virtual ICollection<Chatroom> Chatrooms { get; set; }
    }
}
