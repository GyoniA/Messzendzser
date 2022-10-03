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
        public MessageManager(IDataSource dataSource)
        {

        }
        public void StoreImageMessage(byte[] image, string format, int chatroomId, DB.Models.User user)
        {
            throw new NotImplementedException();
        }

        public void StoreMessage(string message, int chatroomId, DB.Models.User user)
        {
            throw new NotImplementedException();
        }

        public void StoreVoiceMessage(byte[] sound, string format, int chatroomId, DB.Models.User user)
        {
            throw new NotImplementedException();
        }
    }
}
