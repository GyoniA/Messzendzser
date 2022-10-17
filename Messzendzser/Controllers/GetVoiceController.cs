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

namespace Messzendzser.Controllers
{

    /// <summary>
    /// Api endpoint, where users can download uploaded images
    /// Usage:
    ///     Method: Post
    ///     Parameters:
    ///            img: image token 
    /// </summary>
    [Route("api/GetVoice")]
    [ApiController]
    public class GetVoiceController : ControllerBase
    {
        private IDataSource dataSource;
        public GetVoiceController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }
        // POST api/Voice
        [HttpGet()]
        public IActionResult Get( [FromQuery(Name = "voice")] string? voice)
        {            
            IMediaManager mediaManager = new MediaManager(); // TODO
            string? userToken = null;
            Request.Cookies.TryGetValue("user-token", out userToken);
            Utils.FileResult result = LoadSound(voice,userToken,mediaManager);
            if (result.Success)
            {
                return result.FileContentResult;
            }
            else
            {
                return new FileContentResult(Encoding.UTF8.GetBytes(result.ErrorJson),"text/json");
            }
        }
        [NonAction]
        public Utils.FileResult LoadSound(string? soundToken, string? usertoken, IMediaManager media)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();

            #region Sound Token verification
            if (soundToken == null)
            {
                errors.Add("voice", "Voice cannot be empty");
            }
            #endregion

            #region User Token Verification
            UserToken token;

            try
            {
                token = new UserToken(usertoken);
            }
            catch (ArgumentException)
            {
                return new Utils.FileResult(JsonSerializer.Serialize(ResponseMessage<object>.CreateErrorMessage(3, "Invalid user token")));
            }
            #endregion

            Utils.FileResult result = new Utils.FileResult(JsonSerializer.Serialize(ResponseMessage<object>.CreateErrorMessage(4, "Unknown error")));

            if (errors.Count == 0)
            {
                try
                {
                    //Retrieving image
                    string format = "";
                    byte[] image = media.LoadSound(soundToken, out format);
                    result = new Utils.FileResult(new FileContentResult(image, "audio/"+format));
                }// TODO catch image not found exception
                catch (FileNotFoundException ex)
                {
                    errors.Add("error", "Sound not found");
                }
                catch (Exception ex) // Other exception
                {
                    errors.Add("error", ex.Message); // TODO remove for production
                }
            }
            if (errors.Count != 0)
                return new Utils.FileResult(JsonSerializer.Serialize(ResponseMessage<object>.CreateErrorMessage(1, "Invalid parameters", errors)));
            return result;
        }
    }
}
