using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.Message;
using Messzendzser.Model.DB;
using Messzendzser.Model.Managers.Media;
using Microsoft.AspNetCore.Authorization;

namespace Messzendzser.Controllers
{
    /// <summary>
    /// Api endpoint, where users can send text messages
    /// Usage:
    ///     Method: Post
    ///     Parameters:
    ///            chatroomId: username of the new user
    /// </summary>
    [Authorize]
    [Route("api/SendImage")]
    [ApiController]
    public class SendImageController : ControllerBase
    {
        private IDataSource dataSource;
        public SendImageController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        // POST api/SendImage
        [HttpPost(), DisableRequestSizeLimit]
        public ResponseMessage<object> Post([FromHeader(Name = "chatroomId")] string? chatroomId)
        {
            string? userToken = null;
            Request.Cookies.TryGetValue("user-token", out userToken);
            IFormFile? file = Request.Form.Files.GetFile("image");
            return SendImage(file,chatroomId,userToken,new MessageManager(dataSource), null);
        }

        [NonAction]
        public ResponseMessage<object> SendImage(IFormFile? image, string? chatroomId, string? usertoken,IMessageManager messageManager,IMediaManager mediaManager)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();
            int ChatroomId = -1; // Will be reasigned or show an error
            #region Image verification
            string format = "";
            if (image == null)
            {
                errors.Add("message", "Message cannot be empty");
            }
            else
            {
                try
                {
                    format  = FormatOfFile(image);
                }catch(ArgumentException ex) { 
                    errors.Add("image", "Unsupported image fromat");
                }
                
            }
            #endregion

            #region Chatroom Id verification
            if (chatroomId == null)
            {
                errors.Add("chatroomId", "Chatroom id cannot be empty");
            }
            else
            {
                try
                {
                    ChatroomId = Convert.ToInt32(chatroomId);
                }
                catch (Exception ex)
                {
                    errors.Add("chatroomId", "Chatroom id must be a number");
                }
            }
            #endregion

            #region UserTokenVerification
            UserToken token = null;

            try
            {
                token = new UserToken(usertoken);
            }
            catch (Exception)
            {
                errors.Add("usertoken", "Invalid user token");
            }
            #endregion

            if (errors.Count == 0)
            {
                try
                {
                    byte[] imageData;
                    // Record message
                    using (var memoryStream = new MemoryStream())
                    {
                        using (Stream inputStream = image.OpenReadStream()) { 
                            inputStream.CopyTo(memoryStream);
                            imageData = memoryStream.ToArray();
                        }
                    }
                    messageManager.StoreImageMessage(imageData,format, ChatroomId, token.ToUser(),mediaManager);

                }
                catch (Exception ex) // Other exception
                {
                    errors.Add("error", ex.Message); // TODO remove for production
                }
            }

            if (errors.Count != 0) // If there were errors return
                return ResponseMessage<object>.CreateErrorMessage(1, "Invalid parameters", errors);

            //Return OK message            
            return ResponseMessage<object>.CreateOkMessage();
        }

        /// <summary>
        /// Checks the given header against the png header
        /// </summary>
        /// <param name="header">First eight bytes of the file</param>
        /// <returns>True, if the header matches the png header, false otherwise</returns>
        [NonAction]
        private bool isPngHeader(List<byte> header) 
        {
            List<byte> pngHeader = new List<byte>() { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            for(int i = 0;i<8;i++)
                if (!pngHeader.ElementAt(i).Equals(header.ElementAt(i)))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks the given header against the gif header
        /// </summary>
        /// <param name="header">First eight bytes of the file</param>
        /// <returns>True, if the header matches the gif header, false otherwise</returns>
        private bool isGifHeader(List<byte> header)
        {
            List<byte> gifHeader = new List<byte>() { 0x47, 0x49, 0x46, 0x38 };
            for (int i = 0; i < gifHeader.Count; i++)
                if (!gifHeader.ElementAt(i).Equals(header.ElementAt(i)))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks the given header against the jpeg header
        /// </summary>
        /// <param name="firstTwoBytes">First two (or more) bytes of the file</param>
        /// <param name="lastTwoBytes">Last two bytes of the file</param>
        /// <returns>True, if the header matches the jpeg header, false otherwise</returns>
        private bool isJpegHeader(List<byte> firstTwoBytes, List<byte> lastTwoBytes)
        {
            List<byte> jpegFirstBytes = new List<byte>() { 0xff, 0xd8 };
            List<byte> jpegLastBytes = new List<byte>() { 0xff, 0xd9 };
            for (int i = 0; i < 2; i++)
                if (!firstTwoBytes.ElementAt(i).Equals(jpegFirstBytes.ElementAt(i)) || !lastTwoBytes.ElementAt(i).Equals(jpegLastBytes.ElementAt(i)))
                    return false;
            return true;
        }
        /// <summary>
        /// Checks what format an image is in
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string FormatOfFile(IFormFile image)
        {
            List<byte> firstEightBytes = new List<byte>();
            if (image.Length > 10)
            {
                using (Stream stream = image.OpenReadStream())
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int b = stream.ReadByte();
                        if (b != -1)
                            firstEightBytes.Add((byte)b);
                        else
                            throw new ArgumentException();
                    }
                    if (isPngHeader(firstEightBytes))
                        return "png";
                    if (isGifHeader(firstEightBytes))
                        return "gif";
                    stream.Seek(-2, SeekOrigin.End);
                    List<byte> lastTwoBytes = new List<byte>();
                    for (int i = 0; i < 2; i++)
                        lastTwoBytes.Add((byte)stream.ReadByte());
                    if (isJpegHeader(firstEightBytes, lastTwoBytes))
                        return "jpeg";
                    
                }
            }
            throw new ArgumentException();
        }
    }
}
