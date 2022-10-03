using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using System.Text.Json;

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
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IDataSource dataSource;
        public LoginController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }
        // POST api/Login
        [HttpPost()]
        public string Post( [FromHeader(Name = "username")] string? username, [FromHeader(Name = "password")] string? password)
        {            
            IUserManager userManager = new UserManager(dataSource);
            return Login(username, password, userManager);
        }

        public string Login(string? username, string? password, IUserManager userManager)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();

            #region Username verification
            if (username == null)
            {
                errors.Add("username", "Username cannot be empty");
            }
            #endregion

            #region Password verification
            if (password == null)
            {
                errors.Add("password", "Password cannot be empty");
            }
            #endregion

            string token = "";
            if (errors.Count == 0)
            {
                try
                {
                    //Logging user in
                    User user = userManager.LoginUser(username, password);
                    UserToken userToken = new UserToken(user);
                    token = userToken.ToToken();
                }
                catch (WrongCredentialsException ex) // Given credentials don't match any record
                {
                    errors.Add("username", "Given credentials do not match any record");
                }
                catch (Exception ex) // Other exception
                {
                    errors.Add("error", ex.Message); // TODO remove for production
                }
            }
            if (errors.Count != 0)
                return JsonSerializer.Serialize(ResponseMessage.CreateErrorMessage(1, "Invalid parameters", errors));

            // TODO check if all required headers are present
            return JsonSerializer.Serialize(ResponseMessage.CreateOkMessage(new Dictionary<string, string>() { { "token", token } }));
        }
    }
}
