using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using Microsoft.AspNetCore.Identity;

namespace Messzendzser.Model.Managers.User
{
    public class UserManager : IUserManager
    {
        /// <summary>
        /// Data source to use for data operations
        /// </summary>
        private IDataSource dataSource;

        /// <summary>
        /// Constructor for the UserManager class
        /// </summary>
        /// <param name="dataSource">Data source to use for data operations</param>
        public UserManager(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        /// <summary>
        /// Checks username and password against records stored in DataSource
        /// </summary>
        /// <param name="username">Email address or username of the user</param>
        /// <param name="password">Password of the user</param>
        /// <exception cref="WrongCredentialsException">Thrown if given credentials are invalid</exception>
        public DB.Models.User LoginUser(string username, string password)
        {
            DB.Models.User user = dataSource.FindUserByUsernameOrEmail(username);
            if(user != null)
            {
                //Create pass
                PasswordHasher<DB.Models.User> passwordHasher = new PasswordHasher<DB.Models.User>();
                PasswordVerificationResult verificationResult = passwordHasher.VerifyHashedPassword(null, user.Password, password); // Does not use first argument, so it can be left as null
                if(verificationResult.HasFlag(PasswordVerificationResult.Success)|| verificationResult.HasFlag(PasswordVerificationResult.SuccessRehashNeeded))
                {
                    return user;
                }
            }
            throw new WrongCredentialsException();            
        }
        /// <summary>
        /// Authenticates the user in the given DataSource
        /// </summary>
        /// <param name="username">Username or email of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Authentication token</returns>
        /// <exception cref="EmailTakenException">Thrown when the email address </exception>
        public void RegisterUser(string email, string username, string password)
        {
            //Create a password hasher
            Microsoft.AspNetCore.Identity.PasswordHasher<DB.Models.User> passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<DB.Models.User>();
            string hashedPassword = passwordHasher.HashPassword(null, password); // This function doesn't use the first argument, so it can be left as null
            dataSource.CreateUser(email, username, hashedPassword); // Throws exception if unsuccessful
        }
    }
}
