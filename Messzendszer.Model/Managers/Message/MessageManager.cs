using Messzendzser.Model.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.Message
{
    public class MessageManager : IMessageManager
    {
        /// <summary>
        /// Data source to use for data operations
        /// </summary>
        private IDataSource dataSource;
        public MessageManager(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }
        public void StoreImageMessage(byte[] image, string format, int chatroomId, DB.Models.User user)
        {
            string token = "";
            //TODO store image and set token
            dataSource.StoreImageMessage(user.Id, chatroomId, token, format);
            throw new NotImplementedException();
        }

        public void StoreMessage(string message, int chatroomId, DB.Models.User user)
        {
            //TODO check if chatroom and user exists (or don't if it was checked before)
            dataSource.StoreTextMessage(user.Id, chatroomId, message);
        }

        public void StoreVoiceMessage(byte[] sound, string format, int chatroomId, DB.Models.User user)
        {
            string token = "";
            int length = 0;
            //TODO store sound and set token, also set length
            dataSource.StoreVoiceMessage(user.Id, chatroomId, token, length, format);
            throw new NotImplementedException();
        }
    }
}
