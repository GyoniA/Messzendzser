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
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // POST api/Register
        [HttpPost()]
        public string Post( [FromHeader(Name = "username")] string? username, [FromHeader(Name = "password")] string? password)
        {
            //Initialize error list for possible errors
            List<KeyValuePair<string, string>> errors = new List<KeyValuePair<string, string>>();

            #region Username verification
            if (username == null)
            {
                errors.Add(new KeyValuePair<string, string>("username", "Username cannot be empty"));
            }
            #endregion

            #region Password verification
            if (password == null)
            {
                errors.Add(new KeyValuePair<string, string>("password", "Password cannot be empty"));
            }
            #endregion

            string token = "";
            if(errors.Count == 0) { 
                try
                {
                    // Connection to a datasource
                    IDataSource dataSource = new MySQLDatabaseConnection();
                    // Creating a UserManager
                    IUserManager userManager = new UserManager(dataSource);

                    //Logging user in
                    token = userManager.LoginUser(username, password);
                }
                catch (WrongCredentialsException ex) // Given credentials don't match any record
                {
                    errors.Add(new KeyValuePair<string, string>("username", "Given credentials don't match any record"));
                }
                catch (Exception ex) // Other exception
                {
                    errors.Add(new KeyValuePair<string, string>("error", ex.Message)); // TODO remove for production
                }
            }
            if (errors.Count != 0)
                return ResponseMessage.CreateErrorMessage(1, "Invalid parameters", errors.ToArray()).ToJson();

            // TODO check if all required headers are present
            return ResponseMessage.CreateOkMessage(new List<KeyValuePair<string, string>>() { new KeyValuePair<string,string>("token", token) }.ToArray()).ToJson();
        }
    }
}
