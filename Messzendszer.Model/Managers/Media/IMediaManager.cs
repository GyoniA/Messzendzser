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
        public string StoreSound(byte[] sound, string format);
        public byte[] LoadImage(string token, out string format);
        public byte[] LoadSound(string token, out string format);

    }
}
