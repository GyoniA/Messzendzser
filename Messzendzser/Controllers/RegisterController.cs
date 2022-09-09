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
    [Route("api/Register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        // POST api/Register
        [HttpPost()]
        public string Post([FromHeader(Name = "email")] string? email, [FromHeader(Name = "username")] string? username, [FromHeader(Name = "password")] string? password)
        {
            //Initialize error list for possible errors
            List<KeyValuePair<string, string>> errors = new List<KeyValuePair<string, string>>();

            #region Email verification
            if (email == null)
            {
                errors.Add(new KeyValuePair<string, string>("email", "Email cannot be empty"));
            }
            else
            {
                if (!Regex.IsMatch(email, @"([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-z]+)$"))
                {
                    errors.Add(new KeyValuePair<string, string>("email", "Email address must be a valid email address"));
                }
            }
            #endregion

            #region Username verification
            if(username == null)
            {
                errors.Add(new KeyValuePair<string, string>("username", "Username cannot be empty"));
            }
            #endregion

            #region Password verification
            if (password == null)
            {
                errors.Add(new KeyValuePair<string, string>("password", "Password cannot be empty"));
            }//else if() TODO password strength check
            #endregion
            if (errors.Count != 0) { 
                try
                {
                    // Connection to a datasource
                    IDataSource dataSource = new MySQLDatabaseConnection();
                    // Creating a UserManager
                    IUserManager userManager = new UserManager(dataSource);

                    //Registering user
                    userManager.RegisterUser(email, username, password);
                }
                catch (EmailTakenException ex) { // Email is taken exception
                    errors.Add(new KeyValuePair<string, string>("email", "Email is already taken"));
                }
                catch (UsernameTakenException ex) // Username is taken exception
                {
                    errors.Add(new KeyValuePair<string, string>("username", "Username is already taken"));
                }catch (Exception ex) // Other exception
                {
                    errors.Add(new KeyValuePair<string, string>("error", ex.Message)); // TODO remove for production
                }
            }

            if (errors.Count != 0)
                return ResponseMessage.CreateErrorMessage(1, "Invalid parameters",errors.ToArray()).ToJson();

            // TODO check if all required headers are present
            return ResponseMessage.CreateOkMessage().ToJson();
        }
    }
}
