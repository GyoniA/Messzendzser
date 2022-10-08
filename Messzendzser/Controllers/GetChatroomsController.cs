using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using System.Text.Json;
using Messzendzser.Model.Managers.Media;
using System.Text.Unicode;
using System.Text;
using System.Runtime.CompilerServices;
using Messzendzser.Model.Managers.Message;

namespace Messzendzser.Controllers
{

    /// <summary>
    /// Api endpoint, where users can get info about their chatrooms
    /// Usage:
    ///     Method: Post
    /// 
    /// </summary>
    [Route("api/GetChatrooms")]
    [ApiController]
    public class GetChatroomsController : ControllerBase
    {
        private IDataSource dataSource;
        public GetChatroomsController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        // POST api/Login
        [HttpGet()]
        public string Get( )
        {
            string? userToken = null;
            Request.Cookies.TryGetValue("user-token", out userToken);
            IMessageManager messageManager = new MessageManager(dataSource);
            IUserManager userManager = new UserManager(dataSource);
            return GetChatrooms(userToken,userManager);
        }
        [NonAction]
        public string GetChatrooms(string? usertoken,IUserManager userManager)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();

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


            string result = "";
            if (errors.Count == 0)
            {
                try
                {
                    IReadOnlyList<ChatroomInfo> messages = dataSource.GetChatrooms(token.Id);
                    Utils.ResponseMessage.CreateOkMessage(new Dictionary<string, string>() { { "chatrooms", JsonSerializer.Serialize(messages) } });
                }
                catch (Exception ex) // Other exception
                {
                    errors.Add("error", ex.Message); // TODO remove for production
                }
            }
            if (errors.Count != 0)
                return JsonSerializer.Serialize(ResponseMessage.CreateErrorMessage(1, "Invalid parameters", errors));
            return result; 
        }
    }
}
