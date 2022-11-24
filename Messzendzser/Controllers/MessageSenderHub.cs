using Microsoft.AspNetCore.SignalR;

namespace Messzendzser.Controllers
{ 
    public class MessageSenderHub : Hub
    {
        public async Task JoinRoom(string chatroomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
        }

        public async Task LeaveRoom(string chatroomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatroomId);
        }

        public async Task SendMessage(string chatroomId)
        {
            await Clients.Group(chatroomId).SendAsync("ReceiveMessage");
        }
    }
}
