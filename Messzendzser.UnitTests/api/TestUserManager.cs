using Messzendzser.Model.Managers.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.UnitTests.api
{
    class TestUserManager : IUserManager
    {
        public string LoginUser(string username, string password)
        {
            if (username == "wrongusername" || password == "wrongpassword")
                throw new WrongCredentialsException();
            return "SuccessfulToken";
        }

        public void RegisterUser(string email, string username, string password)
        {
            if(username == "takenusername")
            {
                throw new UsernameTakenException();
            }
            if(email == "taken@test.com")
            {
                throw new EmailTakenException();
            }
        }
    }
}
