using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Messzendzser.Controllers
{

    public class WebSocketController : ControllerBase
    {
        [HttpGet("/ws/whiteboard")]
        public async Task Get()
        {
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
        // </snippet>

        private static async Task HandleWhiteboardWebsocket(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);


            while (!receiveResult.CloseStatus.HasValue)
            {
                // TODO Handle message

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

        }
    }
}
