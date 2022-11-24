using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.Message;
using Messzendzser.Model.DB;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Security.Policy;

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
    [Authorize]
    [Route("api/SendMessage")]
    [ApiController]
    public class SendMessageController : ControllerBase
    {
        private IDataSource dataSource;
        public SendMessageController(IDataSource dataSource, MessageSenderHub messageSenderHub)
        {
            this.dataSource = dataSource;
        }

        // POST api/SendMessage
        [HttpPost()]
        public ResponseMessage<object> Post( [FromHeader(Name = "message")] string? message, [FromHeader(Name = "chatroomId")] string? chatroomId)
        {
            return SendMessage(Uri.UnescapeDataString(message),chatroomId,new MessageManager(dataSource));
        }

        [NonAction]
        public ResponseMessage<object> SendMessage(string? message, string? chatroomId,IMessageManager messageManager)
        {
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

            if (errors.Count == 0)
            {
                try
                {
                    // Record message
                    messageManager.StoreMessage(message, ChatroomId, User.ToUser());
                    
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
