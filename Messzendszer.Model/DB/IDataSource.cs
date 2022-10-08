using Messzendzser.Model.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.DB
{
    public interface IDataSource : IDisposable
    {
        public void CreateUser(string email, string username, string password);
        public User GetUser(string username);
        public VoipCredential GetCredentialsForUser(string username);
        public void StoreTextMessage(int userId, int chatroomId, string message);
        public void StoreVoiceMessage(int userId, int chatroomId, string token, int length, string format);
        public void StoreImageMessage(int userId, int chatroomId, string token, string format);
        public int CreateChatroom(int[] users);
        public void AddUserToChatroom(int userId, int chatroomId);
        public bool IsUserInChatroom(int userId, int chatroomId);

        public enum TimeDirecton
        {
            Forward = 0,
            Backward = 1,
        }

        /// <summary>
        /// Searches for <paramref name="count"/> text messages in the chatroom (with id <paramref name="chatroomId"/>), where the sentTime of the message is before or after (dependant on <paramref name="directon"/>) <paramref name="time"/>.
        /// </summary>
        /// <param name="chatroomId">Chatroom to search messages in</param>
        /// <param name="count">Number of messages to be returned</param>
        /// <param name="time">Time to compare messages to</param>
        /// <param name="directon">Direction to look for message compared to <paramref name="time"/></param>
        /// <returns></returns>
        public IReadOnlyList<TextChatMessage> GetTextChatMessages(int chatroomId,int count,DateTime time,TimeDirecton directon);
        
        /// <summary>
        /// Searches for <paramref name="count"/> image messages in the chatroom (with id <paramref name="chatroomId"/>), where the sentTime of the message is before or after (dependant on <paramref name="directon"/>) <paramref name="time"/>.
        /// </summary>
        /// <param name="chatroomId">Chatroom to search messages in</param>
        /// <param name="count">Number of messages to be returned</param>
        /// <param name="time">Time to compare messages to</param>
        /// <param name="directon">Direction to look for message compared to <paramref name="time"/></param>
        /// <returns></returns>
        public IReadOnlyList<ImageChatMessage> GetImageChatMessages(int chatroomId, int count, DateTime time, TimeDirecton directon);
        /// <summary>
        /// Searches for <paramref name="count"/> text messages in the chatroom (with id <paramref name="chatroomId"/>), where the sentTime of the message is before or after (dependant on <paramref name="directon"/>) <paramref name="time"/>.
        /// </summary>
        /// <param name="chatroomId">Chatroom to search messages in</param>
        /// <param name="count">Number of messages to be returned</param>
        /// <param name="time">Time to compare messages to</param>
        /// <param name="directon">Direction to look for message compared to <paramref name="time"/></param>
        /// <returns></returns>
        public IReadOnlyList<VoiceChatMessage> GetVoiceChatMessages(int chatroomId, int count, DateTime time, TimeDirecton directon);
    }
}
