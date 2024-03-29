﻿using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using System.Text.Json;
using Messzendzser.Model.Managers.Media;
using System.Text.Unicode;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Messzendzser.Controllers
{

    /// <summary>
    /// Api endpoint, where users can download uploaded images
    /// Usage:
    ///     Method: Post
    ///     Parameters:
    ///            img: image token 
    /// </summary>
    [Authorize]
    [Route("api/GetImage")]
    [ApiController]
    public class GetImageController : ControllerBase
    {
        private IDataSource dataSource;
        public GetImageController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        // POST api/Login
        [HttpGet()]
        public IActionResult Get( [FromQuery(Name = "img")] string? image)
        {   
            IMediaManager mediaManager = new MediaManager();
            Utils.FileResult result = LoadImage(image,mediaManager);
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
        public Utils.FileResult LoadImage(string? imageToken, IMediaManager media)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();

            #region Image Token verification
            if (imageToken == null)
            {
                errors.Add("image", "Image cannot be empty");
            }
            #endregion

            Utils.FileResult result = new Utils.FileResult(JsonSerializer.Serialize(ResponseMessage<object>.CreateErrorMessage(4,"Unknown error")));

            if (errors.Count == 0)
            {
                try
                {
                    //Retrieving image
                    string format = "";
                    byte[] image = media.LoadImage(imageToken, out format);
                    return new Utils.FileResult(new FileContentResult(image, "image/"+format));
                }
                catch (FileNotFoundException ex) // Image could not be found
                {
                    errors.Add("error", "Image not found");
                }
                catch (Exception ex) // Other exception
                {
                    errors.Add("error", ex.Message); // TODO remove for production
                }
            }
            // Error count has to be greater than 0
            return new Utils.FileResult(JsonSerializer.Serialize(ResponseMessage<object>.CreateErrorMessage(1, "Invalid parameters", errors)));
             
        }
    }
}
