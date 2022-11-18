using Messzendzser.Model.DB.Models;
using Messzendzser.Model.Managers.User;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using System.Security.Cryptography;
using System.Text;

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
        public User GetUser(string username)
        {
            try
            {
                User user = Users.Where(u => u.UserName == username || u.Email == username.ToUpper()).First<User>();
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

        public void StoreTextMessage(int userId, int chatroomId, string message)
        {
            TextChatMessages.Add(new TextChatMessage() { UserId = userId, ChatroomId = chatroomId, Message = message, SentTime = System.DateTime.Now });
            SaveChanges();
        }

        public void StoreVoiceMessage(int userId, int chatroomId, string token, int length, string format)
        {
            VoiceChatMessages.Add(new VoiceChatMessage() { UserId = userId, ChatroomId = chatroomId, Token = token, SentTime = System.DateTime.Now , Format = format, Length = length });
            SaveChanges();
        }

        public void StoreImageMessage(int userId, int chatroomId, string token, string format)
        {
            ImageChatMessages.Add(new ImageChatMessage() { UserId = userId, ChatroomId = chatroomId, Token = token, SentTime = System.DateTime.Now, Format = format });
            SaveChanges();
        }

        public int CreateChatroom(int[] users)
        {
            int ChatroomId = -1;
            using (var dbContextTransaction = Database.BeginTransaction())
            {
                try { 
                    Chatroom chatroom = new Chatroom();
                    Chatrooms.Add(chatroom);
                    SaveChanges();
                    ChatroomId = chatroom.Id;
                    foreach (int userId in users)
                    {
                        chatroom.Users.Add(Users.Where(u => u.Id == userId).First());
                    }
                    dbContextTransaction.Commit();
                    SaveChanges();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                    throw;
                }
            }

            return ChatroomId;
        }

        public void AddUserToChatroom(int userId, int chatroomId)
        {
            Chatroom chatroom = Chatrooms.Where(c => c.Id == chatroomId).First();
            User user = Users.Where(u=>u.Id==userId).First();
            chatroom.Users.Add(user);
            SaveChanges();
        }

        public IReadOnlyList<TextChatMessage> GetTextChatMessages(int chatroomId, int count, DateTime time, IDataSource.TimeDirecton directon)
        {
            List<TextChatMessage> messages = new List<TextChatMessage>();
            var results = TextChatMessages.Where(m =>( 
                m.ChatroomId == chatroomId)
            ).Where(m => directon == IDataSource.TimeDirecton.Forward ? m.SentTime >= time : m.SentTime <= time).OrderByDescending(m=>m.SentTime).Take(count);
            messages.AddRange(results);
            messages.OrderBy(message => message.SentTime);
            return messages;
        }

        public IReadOnlyList<ImageChatMessage> GetImageChatMessages(int chatroomId, int count, DateTime time, IDataSource.TimeDirecton directon)
        {
            List<ImageChatMessage> messages = new List<ImageChatMessage>();
            var results = ImageChatMessages.Where(m =>
                m.ChatroomId == chatroomId
            ).Where(m => directon == IDataSource.TimeDirecton.Forward ? m.SentTime >= time : m.SentTime <= time).OrderByDescending(m => m.SentTime).Take(count);
            messages.AddRange(results);
            messages.OrderBy(message => message.SentTime);
            return messages;
        }

        public IReadOnlyList<VoiceChatMessage> GetVoiceChatMessages(int chatroomId, int count, DateTime time, IDataSource.TimeDirecton directon)
        {
            List<VoiceChatMessage> messages = new List<VoiceChatMessage>();
            var results = VoiceChatMessages.Where(m =>
                m.ChatroomId == chatroomId
            ).Where(m => directon == IDataSource.TimeDirecton.Forward ? m.SentTime >= time : m.SentTime <= time).OrderByDescending(m => m.SentTime).Take(count);
            messages.AddRange(results);
            messages.OrderBy(message => message.SentTime);
            return messages;
        }

        public bool IsUserInChatroom(int userId, int chatroomId)
        {
            return Users.Where(x=>x.Id == userId).Include(x => x.Chatrooms).First().Chatrooms.Where(c=>c.Id== chatroomId).Any();
        }

        public void AddAllAssociations(int userId)
        {
            if (Users.Where(x => x.Id == userId).Any()) { 
                foreach(User user in Users.Where(x => x.Id != userId).ToList())
                {
                    CreateChatroom(new int[] { user.Id, userId });
                }
            }
            else
            {
                throw new ArgumentException("specified user not found");
            }
        }

        public IReadOnlyList<ChatroomInfo> GetChatrooms(int userId)
        {
            if (!Users.Where(x => x.Id == userId).Any()) {
                throw new ArgumentException("UserNotFound");
            }
            User u = Users.Where(x => x.Id == userId).First();
            List <Chatroom> chatrooms = Chatrooms.Where(x=>x.Users.Contains(u)).ToList();
            List<ChatroomInfo> chatroomInfos = new List<ChatroomInfo>();
            foreach (Chatroom chatroom in chatrooms) {
                List<User> assignedUsers = Users.Where(x=>x.Chatrooms.Contains(chatroom)).ToList();
                string name = "";
                foreach (User user in assignedUsers) {
                    if (user.Id != userId)
                        name += $" {user.UserName}";
                }
                chatroomInfos.Add(new ChatroomInfo(name, chatroom.Id));
            }
            return chatroomInfos;
        }

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public void CreateVoipCredentialsForUser(User user)
        {
            VoipCredentials.Add(new VoipCredential() { UserId = user.Id, VoipUsername = user.UserName, VoipPassword = CreateMD5(DateTime.Now.ToString()), VoipRealmHash = "Not needed" });
            SaveChanges();
        }
    }
}
