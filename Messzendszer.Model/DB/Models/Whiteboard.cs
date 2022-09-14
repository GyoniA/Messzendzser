using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Messzendzser.Model.DB.Models
{
    [Table("whiteboard")]
    [MySqlCharSet("latin1")]
    [MySqlCollation("latin1_swedish_ci")]
    public partial class Whiteboard
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("chatroom_id")]
        [StringLength(45)]
        public string ChatroomId { get; set; } = null!;
        [Column("event")]
        [StringLength(400)]
        public string Event { get; set; } = null!;
        [Column("time", TypeName = "datetime")]
        public DateTime Time { get; set; }
    }
}
