using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.Message
{
    public interface ISerializeableMessage
    {
        public string Serialize();
        //public ISerializeableMessage Deserialize(byte[] jsonUTF8);
    }
}
