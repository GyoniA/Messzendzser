using Messzendzser.Model.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.DB
{
    public interface IDataSource
    {
        public void CreateUser(string email, string username, string password);
        public User FindUserByUsernameOrEmail(string username);
    }
}
