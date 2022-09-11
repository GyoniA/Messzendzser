using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.DB.Models
{
    public class Whiteboard
    {
        public int Id { get; set; }
        public int ChatroomId { get; set; }
        public string Event { get; set; }
        public DateTime DateTime { get; set; }

    }
}
