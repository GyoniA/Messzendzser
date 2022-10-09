using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.User
{
    public class ChatroomInfo
    {
        public string Name { get; private set; }
        public int Id { get; private set; }

        public ChatroomInfo(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }
}
