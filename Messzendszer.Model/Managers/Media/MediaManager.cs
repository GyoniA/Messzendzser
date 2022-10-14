using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using static System.Net.Mime.MediaTypeNames;

namespace Messzendzser.Model.Managers.Media
{
    internal class MediaManager : IMediaManager
    {
        public byte[] LoadImage(string token, out string format)
        {
            format = "png";
            string filepath = Directory.GetCurrentDirectory() + @"\Mediamanager\Images\" + token + ".png";
            using (var reader = new BinaryReader(new FileStream(filepath, FileMode.Open, FileAccess.Read)))
            {
                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public byte[] LoadSound(string token, out string format)
        {
            format = "mp3";
            string filepath = Directory.GetCurrentDirectory() + @"\Mediamanager\Sounds\" + token + ".mp3";
            using (var reader = new BinaryReader(new FileStream(filepath, FileMode.Open, FileAccess.Read)))
            {
                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public byte[] LoadWhiteboard(int chatroomId)
        {
            string filepath = Directory.GetCurrentDirectory() + @"\Mediamanager\Whiteboards\board" + chatroomId + ".png";
            using (var reader = new BinaryReader(new FileStream(filepath, FileMode.Open, FileAccess.Read)))
            {
                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public string StoreImage(byte[] image)
        {
            string filename = Guid.NewGuid().ToString();
            string filepath = Directory.GetCurrentDirectory() + @"\Mediamanager\Images\" + filename +".png";
            using (var writer = new BinaryWriter(new FileStream(filepath , FileMode.OpenOrCreate, FileAccess.Write))) { 
                writer.Write(image);
            }
            return filename;
        }

        public string StoreSound(byte[] sound, string format)
        {
            string filename = Guid.NewGuid().ToString();
            string filepath = Directory.GetCurrentDirectory() + @"\Mediamanager\Sounds\" + filename + ".mp3";
            using (var writer = new BinaryWriter(new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                writer.Write(sound);
            }
            return filename;
        }

        public string StoreWhiteboard(byte[] image, int chatroomId)
        {
            string filename = "board" + chatroomId;
            string filepath = Directory.GetCurrentDirectory() + @"\Mediamanager\Whiteboards\" + filename + ".png";
            using (var writer = new BinaryWriter(new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                writer.Write(image);
            }
            return filename;
        }
    }
}
