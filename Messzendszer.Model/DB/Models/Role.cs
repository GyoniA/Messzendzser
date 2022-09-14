using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Messzendzser.Model.DB.Models
{
    [Table("role")]
    public partial class Role
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("role")]
        [StringLength(45)]
        public string Role1 { get; set; } = null!;
    }
}
