using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using Messzendzser.Model.Managers.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.Model.Managers.Message
{
    public class JsonMessageFactory : IMessageFactory
    {
        //Makes a new instance of the requested type of Json message
        public ISerializeableMessage GetMessage(string type)
        {
            //TODO make class work like it should
            switch (type)
            {
                case "text":
                    return new JsonTextMessage();
                case "image":
                    return new JsonImageMessage();
                case "voice":
                    return new JsonVoiceMessage();
                default:
                    throw new Exception("Wrong type. This type of Json message doesn't exist.");
            }
        }
    }
}
