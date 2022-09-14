using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.User
{
    public interface IUserManager
    {
        /// <summary>
        /// Creates and stores a new user int the given DataSource
        /// </summary>
        /// <param name="email">Email address of the user</param>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        /// <exception cref="WrongCredentialsException">Thrown if given credentials are invalid</exception>
        public void RegisterUser(string email, string username, string password);
        /// <summary>
        /// Authenticates the user in the given DataSource
        /// </summary>
        /// <param name="username">Username or email of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Authentication token</returns>
        /// <exception cref="EmailTakenException">Thrown when the email address </exception>
        public DB.Models.User LoginUser(string username, string password);
    }
}
