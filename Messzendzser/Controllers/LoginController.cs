using Messzendzser.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Messzendzser.Model.Managers.User;
using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

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
        private IUserStore<User> userStore;
        private readonly UserManager<User> userManager;
        private SignInManager<User> signInManager;
        public LoginController(IDataSource dataSource, UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager)
        {
            this.dataSource = dataSource;
            this.userManager = userManager;
            this.userStore = userStore;
            this.signInManager = signInManager;
        }
        // POST api/Login
        [HttpPost()]
        public async Task<ResponseMessage<Dictionary<string, string>>> Post( [FromHeader(Name = "username")] string? username, [FromHeader(Name = "password")] string? password)
        {            
            //IUserManager userManager = new UserManager(dataSource);
            return await Login(username, password);
        }
        [NonAction]
        public async Task<ResponseMessage<Dictionary<string, string>>> Login(string? username, string? password)
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

                    var result = await signInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        User user = dataSource.GetUser(username);
                        UserToken userToken = new UserToken(user);
                        token = userToken.ToToken();
                    }
                    //Logging user in
                    /*User user = userManager.LoginUser(username, password);
                    UserToken userToken = new UserToken(user);
                    token = userToken.ToToken();*/
                    return ResponseMessage<Dictionary<string, string>>.CreateOkMessage(new Dictionary<string, string>() { { "token", token } });
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
            // Error count has to be greater than 0
            return ResponseMessage<Dictionary<string, string>>.CreateErrorMessage(1, "Invalid parameters", errors);

        }
    }
}
