using Messzendzser.Model.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Creates and stores a new user int the given DataSource
        /// </summary>
        /// <param name="email">Email address of the user</param>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        /// <exception cref="WrongCredentialsException">Thrown if given credentials are invalid</exception>
        public string LoginUser(string username, string password)
        {
            string token = ""; // TODO
            throw new WrongCredentialsException();
            return token;
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
            dataSource.CreateUser(email, username, password);
            // TODO
            throw new EmailTakenException();
        }
    }
}
