using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;

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
            List<KeyValuePair<string, string>> errors = new List<KeyValuePair<string, string>>();
            int ChatroomId;
            #region Messasge verification
            if (message == null)
            {
                errors.Add(new KeyValuePair<string, string>("message", "Message cannot be empty"));
            }
            #endregion

            #region Chatroom Id verification
            if (chatroomId == null)
            {
                errors.Add(new KeyValuePair<string, string>("chatroomId", "Chatroom id cannot be empty"));
            }
            else
            {
                try {
                    ChatroomId = Convert.ToInt32(chatroomId);
                }catch(Exception ex)
                {
                    errors.Add(new KeyValuePair<string, string>("chatroomId", "Chatroom id must be a number"));
                }
            }
            #endregion
                        
            if(errors.Count == 0) { 
                try
                {
                    // Connection to a datasource
                    IDataSource dataSource = new MySQLDatabaseConnection();
                    // Creating a UserManager
                    IUserManager userManager = new UserManager(dataSource);

                    //Logging user in
                    // TODO add MessageManager call
                }
                catch (Exception ex) // Other exception
                {
                    errors.Add(new KeyValuePair<string, string>("error", ex.Message)); // TODO remove for production
                }
            }
            if (errors.Count != 0)
                return ResponseMessage.CreateErrorMessage(1, "Invalid parameters", errors.ToArray()).ToJson();

            // TODO check if all required headers are present
            return ResponseMessage.CreateOkMessage().ToJson();
        }
    }
}
