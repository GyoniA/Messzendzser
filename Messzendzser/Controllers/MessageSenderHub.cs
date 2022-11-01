using LumiSoft.Net.Mime.vCard;
using Microsoft.AspNet.SignalR;
using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Threading;  
using System.Web;

namespace Messzendzser.Controllers
{ 
    public class MessageSenderHub : Hub
    {
        public Task JoinRoom(string chatroomId)
        {
            return Groups.Add(Context.ConnectionId, chatroomId);
        }

        public Task LeaveRoom(string chatroomId)
        {
            return Groups.Remove(Context.ConnectionId, chatroomId);
        }

        public async Task SendMessage(string message, string chatroomId, string usertoken)
        {
            await Groups.Add(Context.ConnectionId, chatroomId);
            Clients.OthersInGroup(chatroomId).addChatMessage(usertoken, message);
        }
        public async Task SendImageMessage(byte[] imageData, string format, string chatroomId, string usertoken)
        {
            await Groups.Add(Context.ConnectionId, chatroomId);
            Clients.OthersInGroup(chatroomId).addChatMessage(usertoken, imageData, format);
        }
        public async Task SendVoiceMessage(byte[] voiceData, string format, string chatroomId, string usertoken, int length)
        {
            await Groups.Add(Context.ConnectionId, chatroomId);
            Clients.OthersInGroup(chatroomId).addChatMessage(usertoken, voiceData, format, length);
        }
    }
}
