using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.DB
{
    public class MySQLDatabaseConnection : DbContext, IDataSource
    {
        private string connectionString = "Server=sql11.freemysqlhosting.net;Database=sql11517855;Uid=sql11517855;Pwd=SBjJ4urANM;";
        protected override void OnConfiguring(DbContextOptionsBuilder modelBuilder)
        {
            modelBuilder.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
        }

        public void CreateUser(string email, string username, string password)
        {
            throw new NotImplementedException();
        }

        public MySQLDatabaseConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
}
