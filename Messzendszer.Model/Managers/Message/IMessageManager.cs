using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.Message
{
    public interface IMessageManager
    {
        /// <summary>
        /// Stores a text message in DataSource
        /// </summary>
        /// <param name="message">Text message to be stored</param>
        /// <param name="chatroomId">Chatroom for the message to be sent to</param>
        /// <param name="user">User sending the message</param>
        public void StoreMessage(string message, int chatroomId, DB.Models.User user);

        /// <summary>
        /// Stores a voice message in DataSource
        /// </summary>
        /// <param name="sound">Sound to be stored</param>
        /// <param name="format">Encoding of the sound</param>
        /// <param name="chatroomId">Chatroom for the message to be sent to</param>
        /// <param name="user">User sending the message</param>
        public void StoreVoiceMessage(byte[] sound,string format, int chatroomId, DB.Models.User user);

        /// <summary>
        /// Stores an image message in DataSource
        /// </summary>
        /// <param name="image">Image to be stored</param>
        /// <param name="format">Format of the image</param>
        /// <param name="chatroomId">Chatroom for the message to be sent to</param>
        /// <param name="user">User sending the message</param>
        public void StoreImageMessage(byte[] image,string format, int chatroomId, DB.Models.User user);

        /// <summary>
        /// Returns a list of messages to a chatroom that were sent after a certain date
        /// </summary>
        /// <param name="chatroomId">Chatroom to search for messages</param>
        /// <param name="count">Text message to be retrieved</param>
        /// <param name="time">Time after which to search for messages</param>
        /// <param name="direction">Direction in which to search for messages</param>
        public IReadOnlyList<ISerializeableMessage> Update(int chatroomId, int count, DateTime time, IDataSource.TimeDirecton directon);
    }
}
