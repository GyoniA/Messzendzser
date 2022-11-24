namespace Messzendzser.Model.Managers.Media
{
    public class MediaManager : IMediaManager
    {
        public byte[] LoadImage(string token, out string format)
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Mediamanager\Images\", token + "*");
            FileInfo fi = new FileInfo(files[0]);
            format = fi.Extension;
            string filepath = files[0];
            using (var reader = new BinaryReader(new FileStream(filepath, FileMode.Open, FileAccess.Read)))
            {
                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public byte[] LoadSound(string token, out string format)
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Mediamanager\Sounds\", token + "*");
            FileInfo fi = new FileInfo(files[0]);
            format = fi.Extension;
            string filepath = files[0];
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

        public string StoreImage(byte[] image, string format)
        {
            string filename = Guid.NewGuid().ToString();
            string filepath = Directory.GetCurrentDirectory() + @"\Mediamanager\Images\" + filename +"." + format;
            using (var writer = new BinaryWriter(new FileStream(filepath , FileMode.OpenOrCreate, FileAccess.Write))) { 
                writer.Write(image);
            }
            return filename;
        }

        public string StoreSound(byte[] sound, string format)
        {
            string filename = Guid.NewGuid().ToString();
            string filepath = Directory.GetCurrentDirectory() + @"\Mediamanager\Sounds\" + filename + "." + format;
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
