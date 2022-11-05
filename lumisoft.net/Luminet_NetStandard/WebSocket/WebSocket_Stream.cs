using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace Luminet_NetStandard.WebSocket
{
    internal class WebSocket_Stream : Stream
    {
        // Data frame as defined by RFC 6455 section 5
        private class DataFrame{
            public enum Opcodes { 
                ContinuationFrame = 0x00,
                TextFrame = 0x01,
                BinaryFrame = 0x02,
                ConnectionClose = 0x08,
                Ping = 0x09,
                Pong = 0x0A,
            } // Other opcode values are not defined by RFC 6455 section 5.2.

            private byte _fin;
            public byte Fin { get => _fin; set { 
                    if(value>1)
                        throw new ArgumentException("Opcode can only be between 0x00 and 0x08 (3 bit value)");
                    _fin = value;
                } 
            }
            
            private byte _rsv;
            public byte RSV
            {
                get => _rsv;
                set
                {
                    if (value > (byte)0x08)
                        throw new ArgumentException("Opcode can only be between 0x00 and 0x08 (3 bit value)");
                    _opcode = value;
                }
            }

            

            private byte _opcode;
            public byte Opcode { get => _opcode; 
                set { 
                    if (value > (byte)0x0F) 
                        throw new ArgumentException("Opcode can only be between 0x00 and 0x0F (4 bit value)"); 
                    _opcode = value;
                } 
            }

            public byte[] Payload { get; set; }

            internal byte[] Serialize()
            {
                List<byte> result = new List<byte>();

                result.Add((byte)(Fin<<7|RSV<<4|Opcode)); // First bit: Fin, 2-4: RSV, 5-8: Opcode
                // Length depiction according to RFC 6455 section 5.2.
                byte lengthIndicatorByte = (byte)(Payload.Length < 126 ? Payload.Length : (Payload.Length < 65536 ? 126 : 127));

                result.Add(lengthIndicatorByte);
                if (lengthIndicatorByte == 126)
                {// Needed in big endian
                    byte[] lengthBytes = ToBigEndian(BitConverter.GetBytes((ushort)Payload.Length));
                    result.AddRange(lengthBytes);
                }
                else if(lengthIndicatorByte == 127)
                {
                    byte[] lengthBytes = ToBigEndian(BitConverter.GetBytes((ulong)Payload.Length));
                    result.AddRange(lengthBytes);
                }
                result.AddRange(Payload);
                return result.ToArray();
            }
            private byte[] ToBigEndian(byte[] original)
            {
                byte[] result = original;
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(result);
                return result;
            }
        }

        
        private Stream inner;
        private bool isHandshakeComplete = false;

        public WebSocket_Stream(Stream inner)
        {
            this.inner = inner;
        }
        public override bool CanRead => inner.CanRead;

        public override bool CanSeek => inner.CanSeek;

        public override bool CanWrite => inner.CanWrite;

        public override long Length => inner.Length;

        public override long Position { get => inner.Position; set => inner.Position = value; }

        public override void Flush()
        {
            inner.Flush();
        }

        public void StartHandshakeAsServer()
        {
            byte[] buffer = new byte[4096];
            inner.Read(buffer, 0, buffer.Length);
            string s = Encoding.UTF8.GetString(buffer);
            if (Regex.IsMatch(s, "^GET", RegexOptions.IgnoreCase))
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine(s);
                Console.BackgroundColor = ConsoleColor.Black;
                // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                // 3. Compute SHA-1 and Base64 hash of the new value
                // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                string swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);
                string responseString = "HTTP/1.1 101 Switching Protocols\r\n" +
                    "Connection: Upgrade\r\n" +
                    "Upgrade: websocket\r\n" +
                    "Accept-Encoding: identity\r\n" +
                    "Sec-WebSocket-Protocol: sip\r\n" +
                    "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n";
                // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                byte[] response = Encoding.UTF8.GetBytes(responseString);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(responseString);
                Console.BackgroundColor = ConsoleColor.Black;
                inner.Write(response, 0, response.Length);
            }
        }
        private class WebSocketConnectionColosingException : Exception { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                byte[] encoded = new byte[buffer.Length];
                int read = inner.Read(encoded, offset, count);
                byte[] decoded;            
                decoded  = decodeMaskedMessage(encoded);
                Array.Copy(decoded, 0, buffer, offset, decoded.Length);
                string decodedString = Encoding.UTF8.GetString(decoded);
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine(decodedString);
                return decoded.Length;
            }
            catch (WebSocketConnectionColosingException e)
            {
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("WebSocket closing");
                Console.BackgroundColor = ConsoleColor.Black;
                //TODO send closing control frame
                inner.Dispose();
                this.Close();
            }catch(IOException)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine("WebSocket crashed");
                Console.BackgroundColor = ConsoleColor.Black;
                inner.Dispose();
                this.Close();
            }

            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(Encoding.UTF8.GetString(buffer));
            Console.BackgroundColor = ConsoleColor.Black;
            DataFrame dataFrame = new DataFrame();
            dataFrame.Fin = 1;
            dataFrame.RSV = 0;
            dataFrame.Opcode = (byte)DataFrame.Opcodes.TextFrame;
            dataFrame.Payload = buffer;
            byte[] data = dataFrame.Serialize();
            inner.Write(data, offset, data.Length);
        }


        private static byte[] decodeMaskedMessage(byte[] encoded)
        {
            bool fin = (encoded[0] & 0b10000000) != 0,
                    mask = (encoded[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
            int opcode = encoded[0] & 0b00001111, // expecting 1 - text message
                offset = 2;
            ulong msglen =  (ulong)encoded[1] & 0b01111111;

            if ((DataFrame.Opcodes)opcode == DataFrame.Opcodes.ConnectionClose)
                throw new WebSocketConnectionColosingException();

            if (msglen == 126)
            {
                // bytes are reversed because websocket will print them in Big-Endian, whereas
                // BitConverter will want them arranged in little-endian on windows
                msglen = BitConverter.ToUInt16(new byte[] { encoded[3], encoded[2] }, 0);
                offset = 4;
            }
            else if (msglen == 127)
            {
                // To test the below code, we need to manually buffer larger messages — since the NIC's autobuffering
                // may be too latency-friendly for this code to run (that is, we may have only some of the bytes in this
                // websocket frame available through client.Available).
                msglen = BitConverter.ToUInt64(new byte[] { encoded[9], encoded[8], encoded[7], encoded[6], encoded[5], encoded[4], encoded[3], encoded[2] }, 0);
                offset = 10;
            }

            if (msglen == 0)
            {
                return new byte[0];
            }
            else if (mask)
            {
                byte[] decoded = new byte[msglen];
                byte[] masks = new byte[4] { encoded[offset], encoded[offset + 1], encoded[offset + 2], encoded[offset + 3] };
                offset += 4;

                for (ulong i = 0; i < msglen; ++i)
                    decoded[i] = (byte)(encoded[(ulong)offset + i] ^ masks[i % 4]);

                return decoded;
            }
            else
                throw new ArgumentException("mask bit is not set");

        }
    }
}
