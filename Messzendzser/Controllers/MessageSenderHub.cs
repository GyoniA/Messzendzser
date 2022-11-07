using LumiSoft.Net.Mime.vCard;

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Threading;  
using System.Web;

namespace Messzendzser.Controllers
{ 
    public class MessageSenderHub : Hub
    {
        public Task JoinRoom(string chatroomId, string usertoken)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
        }

        public Task LeaveRoom(string chatroomId, string usertoken)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, chatroomId);
        }

        public async Task SendMessage(string message, string chatroomId, string usertoken)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
            await Clients.OthersInGroup(chatroomId).SendAsync("ReceiveMessage", usertoken, message);
        }
        public async Task SendImageMessage(byte[] imageData, string format, string chatroomId, string usertoken)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
            await Clients.OthersInGroup(chatroomId).SendAsync("ReceiveImageMessage", usertoken, imageData, format);
        }
        public async Task SendVoiceMessage(byte[] voiceData, string format, string chatroomId, string usertoken, int length)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
            await Clients.OthersInGroup(chatroomId).SendAsync("ReceiveVoiceMessage", usertoken, voiceData, format, length);
        }
    }
}
