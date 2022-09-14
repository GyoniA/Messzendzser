using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        [HttpPost()]
        public string Post( [FromHeader(Name = "message")] string? message, [FromHeader(Name = "chatroomId")] string? chatroomId)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();
            int ChatroomId;
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
                try {
                    ChatroomId = Convert.ToInt32(chatroomId);
                }catch(Exception ex)
                {
                    errors.Add("chatroomId", "Chatroom id must be a number");
                }
            }
            #endregion
                        
            if(errors.Count == 0) { 
                try
                {
                    // Connection to a datasource
                    IDataSource dataSource = new MySQLDbConnection();
                    // Creating a UserManager
                    IUserManager userManager = new UserManager(dataSource);

                    //Logging user in
                    // TODO add MessageManager call
                }
                catch (Exception ex) // Other exception
                {
                    errors.Add("error", ex.Message); // TODO remove for production
                }
            }
            if (errors.Count != 0)
                return JsonSerializer.Serialize(ResponseMessage.CreateErrorMessage(1, "Invalid parameters", errors));

            // TODO check if all required headers are present
            return JsonSerializer.Serialize(ResponseMessage.CreateOkMessage());
        }
    }
}
