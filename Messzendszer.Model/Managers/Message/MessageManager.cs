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
            string token = manager.StoreImage(image, format);
            dataSource.StoreImageMessage(user.Id, chatroomId, token, format);
        }

        public void StoreMessage(string message, int chatroomId, DB.Models.User user)
        {
            dataSource.StoreTextMessage(user.Id, chatroomId, message);
        }

        public void StoreVoiceMessage(byte[] sound, string format, int chatroomId, DB.Models.User user, int length, IMediaManager manager)
        {
            string token = manager.StoreSound(sound, format);
            dataSource.StoreVoiceMessage(user.Id, chatroomId, token, length, format);
        }

        public IReadOnlyList<Message> Update(int chatroomId, int count, DateTime time, IDataSource.TimeDirecton direction)
        {
            IReadOnlyList<TextChatMessage> texts = dataSource.GetTextChatMessages(chatroomId, count, time, direction);
            IReadOnlyList<ImageChatMessage> images = dataSource.GetImageChatMessages(chatroomId, count, time, direction);
            IReadOnlyList<VoiceChatMessage> voices = dataSource.GetVoiceChatMessages(chatroomId, count, time, direction);

            int numberOfMessages = texts.Count + images.Count + voices.Count;
            if (count > numberOfMessages)
            {
                count = numberOfMessages;
            }
            List<TextMessage> Texts = texts.Select(x => new TextMessage(x)).ToList();
            List<ImageMessage> Images = images.Select(x => new ImageMessage(x)).ToList();
            List<VoiceMessage> Voices = voices.Select(x => new VoiceMessage(x)).ToList();
            List<Message> combined = new List<Message>();
            combined.AddRange(Texts);
            combined.AddRange(Images);
            combined.AddRange(Voices);
            combined = combined.OrderBy(x => x.Time).ToList();
            
            List<Message> parentList = combined.Take(count).Cast<Message>().ToList();
            return parentList;
        }
    }
}
