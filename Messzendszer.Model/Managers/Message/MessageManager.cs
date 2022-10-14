using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using Messzendzser.Model.Managers.Media;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubiety.Dns.Core.Common;

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
        public void StoreImageMessage(byte[] image, string format, int chatroomId, DB.Models.User user, IMediaManager manager)
        {
            string token = manager.StoreImage(image);
            dataSource.StoreImageMessage(user.Id, chatroomId, token, format);
            throw new NotImplementedException();
        }

        public void StoreMessage(string message, int chatroomId, DB.Models.User user)
        {
            //TODO check if chatroom and user exists (or don't if it was checked before)
            dataSource.StoreTextMessage(user.Id, chatroomId, message);
        }

        public void StoreVoiceMessage(byte[] sound, string format, int chatroomId, DB.Models.User user, int length, IMediaManager manager)
        {
            string token = manager.StoreSound(sound, format);
            dataSource.StoreVoiceMessage(user.Id, chatroomId, token, length, format);
            throw new NotImplementedException();
        }

        public IReadOnlyList<ISerializeableMessage> Update(int chatroomId, int count, DateTime time, IDataSource.TimeDirecton direction)
        {
            IReadOnlyList<TextChatMessage> texts = dataSource.GetTextChatMessages(chatroomId, count, time, direction);
            IReadOnlyList<ImageChatMessage> images = dataSource.GetImageChatMessages(chatroomId, count, time, direction);
            IReadOnlyList<VoiceChatMessage> voices = dataSource.GetVoiceChatMessages(chatroomId, count, time, direction);

            int numberOfMessages = texts.Count + images.Count + voices.Count;
            if (count > numberOfMessages)
            {
                count = numberOfMessages;
            }
            List<TextMessage> jsonTexts = texts.Select(x => new TextMessage(x)).ToList();
            List<ImageMessage> jsonImages = images.Select(x => new ImageMessage(x)).ToList();
            List<VoiceMessage> jsonVoices = voices.Select(x => new VoiceMessage(x)).ToList();
            List<Message> combined = new List<Message>();
            combined.AddRange(jsonTexts);
            combined.AddRange(jsonImages);
            combined.AddRange(jsonVoices);
            combined = combined.OrderBy(x => x.Time).ToList();
            return (IReadOnlyList<ISerializeableMessage>)combined.Take(count);
        }
    }
}
