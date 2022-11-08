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
        public Task JoinRoom(string chatroomId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
        }

        public Task LeaveRoom(string chatroomId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, chatroomId);
        }

        public async Task SendMessage(string chatroomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
            await Clients.Group(chatroomId).SendAsync("ReceiveMessage");
        }


        /*
        public async Task SendMessage(string message, string chatroomId, int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
            await Clients.OthersInGroup(chatroomId).SendAsync("ReceiveMessage", userId, message);
        }
        public async Task SendImageMessage(string token, string chatroomId, int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
            await Clients.OthersInGroup(chatroomId).SendAsync("ReceiveImageMessage", userId, token);
        }
        public async Task SendVoiceMessage(string token, string chatroomId, int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
            await Clients.OthersInGroup(chatroomId).SendAsync("ReceiveVoiceMessage", userId, token);
        }*/
    }
}
