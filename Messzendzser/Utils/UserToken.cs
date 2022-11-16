using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Messzendzser.Utils
{
    public class UserToken
    {
        private const string key = "78e73a77bc9ffb1ca9f72ca1351c0acfaa9bdea6c261c66323c20b512fb6a1feae43d228b92150cde7e128610c7e5b3d59448d7500f336c0bdba5797c6eabfd07be9911b21a0ff1b1e22b4b5cf88c051d2fbc30988377f7fce2a695b918181f4dbe95a3a46d925de145f8b888f4459a10333148cd42f69489d9dfdfb3de42b11e09cc06002addd0addce05c30a92ae47fb4177fa11decbec119520d10d1406b116b9eabe5cdbb210ea0b64bd2e515278b4cc6da0680fed3213172521baf86182a9d1e8954752fe997236f88b3f95598f6d50744a131e3c5b4d35aa8a5cabb7a70a0282dbfbe5d49457bbad8761e64cf274c3a428c966616b433d4e057f696e820c0986aa47d5e89b44393f5a4cc7e266141261394c906e438e89f0bd33ca6a0174ff7e00ec8d91bacc9ec566";

        public int Id { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string VoipPassword { get; private set; }
        public DateTime Created { get; private set; }

        public User ToUser()
        {
            User user = new User();
            user.Id = Id;
            user.Username = Username;
            user.Email = Email;
            return user;
        }

        /// <summary>
        /// Creates the signed token of the UserToken instance
        /// </summary>
        /// <returns>Signed token</returns>
        public string ToToken()
        {
            // Create secutity key
            SymmetricSecurityKey securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            // Create credentials
            SigningCredentials credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            // Create header
            JwtHeader header = new JwtHeader(credentials);
            // Create payload
            JwtPayload payload = new JwtPayload(){ 
                { "Id", Id } ,
                { "Username" , Username },
                { "Email" , Email },
                { "VoipPassword" , VoipPassword },
                { "Created" , Created },
            };

            // Create token
            var token = new JwtSecurityToken(header, payload);
            // Create handler to expot
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }

        /// <summary>
        /// Creates a new instance of the UserToken class
        /// </summary>
        /// <param name="Id">Id of the user</param>
        /// <param name="Username">Username of the user</param>
        /// <param name="Email">Email of the user</param>
        public UserToken(int Id, string Username, string Email,string VoipPassword)
        {
            this.Id = Id;
            this.Username = Username;
            this.Email = Email;
            this.VoipPassword = VoipPassword;
            Created = DateTime.Now;
        }

        /// <summary>
        /// Creates a new instance of the UserToken class
        /// </summary>
        /// <param name="user">User object containing data about the user</param>
        public UserToken(User user,IDataSource dataSource)
        {
            this.Id = user.Id;
            this.Username = user.Username;
            this.Email = user.Email;
            this.VoipPassword = dataSource.GetCredentialsForUser(Username).VoipPassword;
            Created = DateTime.Now;
        }


        /// <summary>
        /// Creates a new instance of the UserToken class
        /// </summary>
        /// <param name="token">Signed JWT token containing user data</param>
        public UserToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtPayload payload = handler.ReadJwtToken(token).Payload;
            object? id;
            object? username,email;
            object? created;

            payload.TryGetValue("Id",out id);
            payload.TryGetValue("Username",out username);
            payload.TryGetValue("Email",out email);
            payload.TryGetValue("Created",out created);

            if(id==null||username == null|| email==null|| created == null)
            {
                throw new ArgumentException("Given token did not contain every necesary information");
            }

            Id = Convert.ToInt32((Int64)id);
            Username = (string)username;
            Email = (string)email;
            Created = (DateTime)created;

        }
    }
}
