using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.Media
{
    public class DataFile
    {
        public string Format { get; set; } = null!;
        public Byte[] Data { get; set; } = null!;

        public DataFile(string format, byte[] data)
        {
            this.Format = format;
            this.Data = data;
        }
    }
}
