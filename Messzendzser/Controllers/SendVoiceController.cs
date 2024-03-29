﻿using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
using System.Text.Json;
using System.Text.Json.Serialization;
using Messzendzser.Model.Managers.Message;
using Messzendzser.Model.Managers.Media;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    [Route("api/SendVoice")]
    [ApiController]
    public class SendVoiceController : ControllerBase
    {
        private IDataSource dataSource;

        private const int BUFFER_SIZE = 6000000;
        public SendVoiceController(IDataSource dataSource, MessageSenderHub messageSenderHub)
        {
            this.dataSource = dataSource;
        }

        // POST api/Register
        [HttpPost(),DisableRequestSizeLimit]
        public ResponseMessage<object> Post( [FromHeader(Name = "format")] string? format, [FromHeader(Name = "chatroomId")] string? chatroomId, [FromHeader(Name = "length")] string? length)
        {
            byte[] buffer = new byte[(int)Request.ContentLength];
            Request.Body.ReadAsync(buffer, 0, buffer.Length);
            return SendVoice(buffer, format, chatroomId,length, new MessageManager(dataSource), new MediaManager());
        }

        [NonAction]
        public ResponseMessage<object> SendVoice(byte[]? voice,string? format, string? chatroomId,string? length, IMessageManager messageManager, IMediaManager mediaManager)
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


            if (errors.Count == 0)
            {
                try
                {
                    messageManager.StoreVoiceMessage(voice, format, ChatroomId, User.ToUser(), Length, mediaManager);

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
