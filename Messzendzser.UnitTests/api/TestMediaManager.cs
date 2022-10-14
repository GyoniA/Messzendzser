using Messzendzser.Model.Managers.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.UnitTests.api
{
    internal class TestMediaManager : IMediaManager
    {
        public byte[] LoadImage(string token, out string format)
        {
            if(token == "valid")
            {
                format = "jpg";
                return new byte[4] { 1,2,3,4 };
            }
            throw new FileNotFoundException("Image not found");
        }

        public byte[] LoadSound(string token, out string format)
        {
            if (token == "valid")
            {
                format = "mp3";
                return new byte[4] { 1, 2, 3, 4 };
            }
            throw new FileNotFoundException("Image not found");
        }

        public byte[] LoadWhiteboard(int chatroomId)
        {
            throw new NotImplementedException();
        }

        public string StoreImage(byte[] image)
        {
            return "token";
        }

        public string StoreSound(byte[] sound, string format)
        {
            return format;
        }

        public string StoreWhiteboard(byte[] image, int chatroomId)
        {
            throw new NotImplementedException();
        }
    }
}
