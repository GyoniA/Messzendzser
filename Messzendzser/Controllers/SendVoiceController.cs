using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
using System.Text.Json;
using System.Text.Json.Serialization;
using Messzendzser.Model.Managers.Message;
using Messzendzser.Model.Managers.Media;

namespace Messzendzser.Controllers
{

    /// <summary>
    /// Api endpoint, where users can send their registration requests
    /// Usage:
    ///     Method: Post
    ///     Parameters:
    ///            format: format of the uploaded audio file
    ///            chatroomId: id of the chatroom to send the message to
    ///            length: length of the audio message
    /// </summary>
    [Route("api/SendVoice")]
    [ApiController]
    public class SendVoiceController : ControllerBase
    {
        private IDataSource dataSource;
        public SendVoiceController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        // POST api/Register
        [HttpPost(),DisableRequestSizeLimit]
        public ResponseMessage<object> Post( [FromHeader(Name = "format")] string? format, [FromHeader(Name = "chatroomId")] string? chatroomId, [FromHeader(Name = "length")] string? length)
        {
            string? userToken = null;
            Request.Cookies.TryGetValue("user-token", out userToken);
            IFormFile? file = Request.Form.Files.GetFile("voice");
            return SendVoice(file, format, chatroomId,length, userToken, new MessageManager(dataSource), new MediaManager());
        }

        [NonAction]
        public ResponseMessage<object> SendVoice(IFormFile? voice,string? format, string? chatroomId,string? length, string? usertoken, IMessageManager messageManager, IMediaManager mediaManager)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();
            int ChatroomId = -1; // Will be reasigned or show an error
            int Length = -1;

            #region Format verification
            if (format == null)
                errors.Add("format", "Format cannot be empty");
            else 
                if (!(format == "ogg" || (format == "MP3")))
                    errors.Add("format", "Unsupported format");
            
            #endregion

            #region Voice verification            
            if (voice == null)            
                errors.Add("voice", "Voice cannot be empty");            
            #endregion

            #region Chatroom Id verification
            if (chatroomId == null)
            {
                errors.Add("chatroomId", "ChatroomId id cannot be empty");
            }
            else
            {
                try
                {
                    ChatroomId = Convert.ToInt32(chatroomId);
                }
                catch (Exception ex)
                {
                    errors.Add("chatroomId", "ChatroomId id must be a number");
                }
            }
            #endregion

            #region Length verification
            if (length == null)
            {
                errors.Add("length", "length cannot be empty");
            }
            else
            {
                try
                {
                    Length = Convert.ToInt32(length);
                }
                catch (Exception ex)
                {
                    errors.Add("length", "length must be a number");
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
                    byte[] voiceData;
                    // Record message
                    using (var memoryStream = new MemoryStream())
                    {
                        using (Stream inputStream = voice.OpenReadStream())
                        {
                            inputStream.CopyTo(memoryStream);
                            voiceData = memoryStream.ToArray();
                        }
                    }
                    messageManager.StoreVoiceMessage(voiceData, format, ChatroomId, token.ToUser(),Length, mediaManager);

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
    }
}
