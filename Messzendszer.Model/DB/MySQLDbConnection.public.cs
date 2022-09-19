using Messzendzser.Model.DB.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Messzendzser.Model.DB
{
    // Scaffold command: Scaffold-DbContext "Server=localhost;Port=3306;Database=messzendzser;Uid=root;Pwd=secret;" Pomelo.EntityFrameworkCore.MySql -OutputDir DB\Models -ContextDir DB -Context MySQLDbConnection -Force -DataAnnotations
    public partial class MySQLDbConnection : IDataSource
    {
        public void CreateUser(string email, string username, string password)
        {
            try
            {
                this.Database.ExecuteSqlInterpolated($"call messzendzser.register_user({email}, {username}, {password});");
            }
            catch (MySqlException ex)
            {
                switch (ex.SqlState)
                {
                    case "50001":
                        throw new Managers.User.EmailTakenException();
                    case "50002":
                        throw new Managers.User.UsernameTakenException();
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Finds a user in DataSource by searching for their username or email address
        /// </summary>
        /// <param name="username">Username or email of user</param>
        /// <returns>User identified if found, null otherwise</returns>
        public User FindUserByUsernameOrEmail(string username)
        {
            try
            {
                User user = Users.Where(u => u.Username == username || u.Email == username).First<User>();
                return user;
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }
        
        
        public VoipCredential GetCredentialsForUser(string username)
        {
            VoipCredential creds = VoipCredentials.Where(s => s.VoipUsername == username).First();
            if (creds == null)
                throw new ArgumentException("No user with given username");
            return creds;
        }
    }
}
