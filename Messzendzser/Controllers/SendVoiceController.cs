using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
using System.Text.Json;
using System.Text.Json.Serialization;
using Messzendzser.Model.Managers.Message;

namespace Messzendzser.Controllers
{

    /// <summary>
    /// Api endpoint, where users can send their registration requests
    /// Usage:
    ///     Method: Post
    ///     Parameters:
    ///            email: email adress of the new user
    ///            username: username of the new user
    ///            password: password of the new user
    /// </summary>
    [Route("api/SendVoice")]
    [ApiController]
    public class SendVoiceController : ControllerBase
    {
        // POST api/Register
        [HttpPost(),DisableRequestSizeLimit]
        public string Post( [FromHeader(Name = "format")] string? format, [FromHeader(Name = "chatroomId")] string? chatroomId)
        {
            string? userToken = null;
            Request.Cookies.TryGetValue("user-token", out userToken);
            IFormFile? file = Request.Form.Files.GetFile("voice");
            return SendVoice(file, chatroomId, userToken);
        }

        public string SendVoice(IFormFile? voice,string? format, string? chatroomId, string? usertoken)
        {
            if (usertoken == null)
                return JsonSerializer.Serialize(ResponseMessage.CreateErrorMessage(2, "No user token given"));
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();
            int ChatroomId = -1; // Will be reasigned or show an error
            #region Format verification
            if (format == null)
                errors.Add("format", "Format cannot be empty");
            else
                if (!(format == "ogg"))
                    errors.Add("format", "Unsupported format");
            #endregion

            #region Voice verification            
            if (voice == null)            
                errors.Add("voice", "Voice cannot be empty");            
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
            UserToken token;

            try
            {
                token = new UserToken(usertoken);
            }
            catch (ArgumentException)
            {
                return JsonSerializer.Serialize(ResponseMessage.CreateErrorMessage(3, "Invalid user token"));
            }
            #endregion

            if (errors.Count == 0)
            {
                try
                {
                    // Connection to a datasource
                    IDataSource dataSource = new MySQLDbConnection();
                    // Creating a MessageManager
                    IMessageManager messageManager = new MessageManager();

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
                    messageManager.StoreVoiceMessage(voiceData, format, ChatroomId, token.ToUser());

                }
                catch (Exception ex) // Other exception
                {
                    errors.Add("error", ex.Message); // TODO remove for production
                }
            }

            if (errors.Count != 0) // If there were errors return
                return JsonSerializer.Serialize(ResponseMessage.CreateErrorMessage(1, "Invalid parameters", errors));

            //Return OK message            
            return JsonSerializer.Serialize(ResponseMessage.CreateOkMessage());
        }
    }
}
