using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.User
{
    public class UserAuthentaticationToken
    {
        public int UserId { get; }
        public string Username { get; }
        public string Email { get; }

        public UserAuthentaticationToken(int userId, string username, string email)
        {
            UserId = userId;
            Username = username;
            Email = email;
        }

        public string ToToken() {
            return "token"; // TODO
        }

        public static UserAuthentaticationToken FromToken(string token) {
            // TODO
            return new UserAuthentaticationToken(1, "username", "email");
        }

    }
}
