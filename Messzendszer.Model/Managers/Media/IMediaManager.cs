using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.Media
{
    public interface IMediaManager
    {
        public string StoreImage(byte[] image);
        public string StoreSound(byte[] sound);
        public byte[] LoadImage(string token);
        public byte[] LoadSound(string token);

    }
}
