using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.DB.Models
{
    public class VoiceMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public string Token { get; set; }
        public int Length { get; set; }
    }
}
