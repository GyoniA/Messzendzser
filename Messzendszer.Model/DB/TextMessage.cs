using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.DB
{
    public class TextMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public DateTime timestamp { get; set; }
        public string Message { get; set; }
    }
}
