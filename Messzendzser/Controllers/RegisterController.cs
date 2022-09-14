using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
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
    [Route("api/Register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        // POST api/Register
        [HttpPost()]
        public string Post([FromHeader(Name = "email")] string? email, [FromHeader(Name = "username")] string? username, [FromHeader(Name = "password")] string? password)
        {
            IDataSource dataSource = new MySQLDbConnection();
            IUserManager userManager = new UserManager(dataSource);
            return Register(email,username,password,userManager);
        }

        public string Register(string? email,string? username,string? password, IUserManager userManager)
        {
            //Initialize error list for possible errors
            Dictionary<string, string> errors = new Dictionary<string, string>();

            #region Email verification
            if (email == null)
            {
                errors.Add("email", "Email cannot be empty");
            }
            else
            {
                if (!Regex.IsMatch(email, @"([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-z]+)$"))
                {
                    errors.Add("email", "Email address must be a valid email address");
                }
            }
            #endregion

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
            else if (!Regex.IsMatch(password, @"^[a-zA-Z0-9~`! @#$%^&*()_\-+={[}\]|\:;'<,>.?/]{8,}$"))
            {
                errors.Add("password", "Password is not strong enough");
            }
            #endregion
            if (errors.Count == 0)
            {
                try
                {
                    //Registering user
                    userManager.RegisterUser(email, username, password);
                }
                catch (EmailTakenException ex)
                { // Email is taken exception
                    errors.Add("email", "Email is already taken");
                }
                catch (UsernameTakenException ex) // Username is taken exception
                {
                    errors.Add("username", "Username is already taken");
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
