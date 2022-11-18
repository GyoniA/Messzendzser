using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Messzendzser.Model.DB.Models;

// Add profile data for application users by adding properties to the MesszendzserIdentityUser class
public class User : IdentityUser<int>
{
    [InverseProperty("User")]
    public virtual VoipCredential? VoipCredential { get; set; }
    [InverseProperty("User")]
    public virtual ICollection<ImageChatMessage> ImageChatMessages { get; set; } = null!;
    [InverseProperty("User")]
    public virtual ICollection<TextChatMessage> TextChatMessages { get; set; } = null!;
    [InverseProperty("User")]
    public virtual ICollection<VoiceChatMessage> VoiceChatMessages { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Chatroom> Chatrooms { get; set; } = null!;
}

