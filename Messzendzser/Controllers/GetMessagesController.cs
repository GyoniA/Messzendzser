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
using Microsoft.AspNetCore.Authorization;

namespace Messzendzser.Controllers
{

    /// <summary>
    /// Api endpoint, where users can get messages
    /// Usage:
    ///     Method: Post
    ///     Parameters:
    ///            chatroomId: Id of the chatroom
    ///            count: number of messages to return
    ///            time: datetime to compare messages to (yyyy-MM-dd HH:mm:ss)
    ///            dir: direction to search messages ("forward" or "backward")
    /// 
    /// </summary>
    [Authorize]
    [Route("api/GetMessages")]
    [ApiController]
    public class GetMessagesController : ControllerBase
    {
        private IDataSource dataSource;
        public GetMessagesController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        // POST api/Login
        [HttpGet()]
        public ResponseMessage<IReadOnlyList<object>> Get( [FromHeader(Name = "chatroomId")] string? chatroom, [FromHeader(Name = "count")] string? count, [FromHeader(Name = "time")] string? time,[FromHeader(Name = "dir")] string? dir)
        {
            IMessageManager messageManager = new MessageManager(dataSource);
            IUserManager userManager = new UserManager(dataSource);
            return FetchMessages(chatroom,count,time,dir, messageManager, userManager);
        }
        [NonAction]
        public ResponseMessage<IReadOnlyList<object>> FetchMessages(string? chatroom,string? count, string? time, string? dir, IMessageManager messageManager,IUserManager userManager)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();
            int chatroomId = -1;
            int messageCount = -1;
            DateTime dateTime = new DateTime();
            IDataSource.TimeDirecton timeDirecton = IDataSource.TimeDirecton.Forward;


            #region Chatroom
            if (chatroom == null)
            {
                errors.Add("chatroomId", "ChatroomId cannot be empty");
            }
            else
            {
                try
                {
                    chatroomId = Convert.ToInt32(chatroom);
                }
                catch (Exception ex)
                {
                    errors.Add("chatroomId", ex.Message);
                }

            }
            #endregion

            #region Count
            if (count == null)
            {
                errors.Add("count", "count cannot be empty");
            }
            else
            {
                try
                {
                    messageCount = Convert.ToInt32(count);
                }
                catch (Exception ex)
                {
                    errors.Add("count", ex.Message);
                }

            }
            #endregion

            #region Time
            if (time == null)
            {
                errors.Add("time", "time cannot be empty");
            }
            else
            {
                try
                {
                    dateTime = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    errors.Add("time", ex.Message);
                }

            }
            #endregion

            #region Direction
            if (dir == null)
            {
                errors.Add("dir", "dir cannot be empty");
            }
            else
            {
                if (dir == "forward")
                {
                    timeDirecton = IDataSource.TimeDirecton.Forward;
                }
                else if (dir == "backward")
                {
                    timeDirecton = IDataSource.TimeDirecton.Backward;
                }
                else
                {
                    errors.Add("dir", "dir has to be either forward or backward");
                }
            }
            #endregion



            if (errors.Count == 0)
            {
                try
                {
                    IReadOnlyList<Message> messages = messageManager.Update(chatroomId, messageCount, dateTime, timeDirecton);
                    return ResponseMessage<IReadOnlyList<object>>.CreateOkMessage(messages.Select(x=> (object)x).ToList());
                }
                catch (Exception ex) // Other exception
                {
                    errors.Add("error", ex.Message); // TODO remove for production
                }
            }
            // Error count has to be greater than 0
            return ResponseMessage<IReadOnlyList<object>>.CreateErrorMessage(1, "Invalid parameters", errors);
            
        }
    }
}
