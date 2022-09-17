using Messzendzser.Model.DB.Models;
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
        public User LoginUser(string username, string password)
        {
            if (username == "wrongusername" || password == "wrongpassword")
                throw new WrongCredentialsException();
            return new User() { Id = 0, Email = "testemail@test.com", Username = "testusername" };
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
