using Messzendzser.Model.DB;
using Messzendzser.WhiteBoard;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Messzendzser.Controllers
{

    public class WebSocketController : ControllerBase
    {
        private readonly IDataSource dataSource;

        public WebSocketController(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        static WhiteboardManager whiteboardManager = null;
        [HttpGet("/ws/whiteboard")]
        public async Task Get()
        {
            if(whiteboardManager == null)
                whiteboardManager = new WhiteboardManager(dataSource);            
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
