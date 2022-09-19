using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Messzendzser.Model.DB.Models
{
    [Table("voip_credentials")]
    public partial class VoipCredential
    {
        [Key]
        [Column("user_id", TypeName = "int(11)")]
        public int UserId { get; set; }
        [Column("voip_username")]
        [StringLength(60)]
        public string VoipUsername { get; set; } = null!;
        [Column("voip_password")]
        [StringLength(32)]
        public string VoipPassword { get; set; } = null!;
        [Column("voip_realm_hash")]
        [StringLength(32)]
        public string VoipRealmHash { get; set; } = null!;

        [ForeignKey("UserId")]
        [InverseProperty("VoipCredential")]
        public virtual User User { get; set; } = null!;
    }
}
