using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.Message;
using Messzendzser.Model.DB;
using System.Text.Json;

namespace Messzendzser.Controllers
{

    /// <summary>
    /// Api endpoint, where users can send text messages
    /// Usage:
    ///     Method: Post
    ///     Parameters:
    ///            message: email adress of the new user
    ///            chatroomId: username of the new user
    /// </summary>
    [Route("api/SendMessage")]
    [ApiController]
    public class SendMessageController : ControllerBase
    {
        private IDataSource dataSource;
        public SendMessageController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        // POST api/SendMessage
        [HttpPost()]
        public string Post( [FromHeader(Name = "message")] string? message, [FromHeader(Name = "chatroomId")] string? chatroomId)
        {
            string? userToken = null;
            Request.Cookies.TryGetValue("user-token", out userToken);
            return SendMessage(message,chatroomId,userToken,new MessageManager(dataSource));
        }

        [NonAction]
        public string SendMessage(string? message, string? chatroomId, string? usertoken,IMessageManager messageManager)
        {
            if(usertoken == null)
                return JsonSerializer.Serialize(ResponseMessage.CreateErrorMessage(2, "No user token given"));
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();
            int ChatroomId = -1; // Will be reasigned or show an error
            #region Messasge verification
            if (message == null)
            {
                errors.Add("message", "Message cannot be empty");
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
                    // Record message
                    messageManager.StoreMessage(message, ChatroomId, token.ToUser());

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
