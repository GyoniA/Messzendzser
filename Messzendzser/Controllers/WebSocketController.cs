using Messzendzser.WhiteBoard;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Messzendzser.Controllers
{

    public class WebSocketController : ControllerBase
    {
        WhiteboardManager whiteboardManager;
        [HttpGet("/ws/whiteboard")]
        public async Task Get(WhiteboardManager whiteboardManager)
        {
            this.whiteboardManager = whiteboardManager;
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await HandleWhiteboardWebsocket(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task HandleWhiteboardWebsocket(WebSocket webSocket)
        {
            await whiteboardManager.AcceptConnection(webSocket);
        }
    }
}
