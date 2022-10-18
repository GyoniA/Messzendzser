namespace Messzendzser.WhiteBoard
{
    using Messzendzser.Model.DB.Models;
    using Microsoft.Extensions.Logging;
    using Org.BouncyCastle.Utilities;
    using SkiaSharp;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Timers;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

    public class WhiteboardManager : IWhiteboardManager
    {
        enum State
        {
            NewConnection,
            Authenticated
        }

        private int waitTime = 5000;
        
        //stores each chatrooms whiteboard
        private ConcurrentDictionary<int, Whiteboard> whiteboards;


        private ConcurrentDictionary<WhiteboardConnection, DateTime> lastTimestamps;

        private CancellationTokenSource stop = new CancellationTokenSource();

        public async Task AcceptConnection(WebSocket socket)
        {
            await ClientLoop(socket);
        }

        public WhiteboardManager()
        {
            whiteboards = new ConcurrentDictionary<int, Whiteboard>();
            lastTimestamps = new ConcurrentDictionary<WhiteboardConnection, DateTime>();            
        }
        public static bool AuthenticateMessage(WhiteboardAuthenticationMessage wm)
        {
            //TODO implement authentication
            /*authenticate with:
                private string username;
                private string password;
                private Chatroom chatroom;*/
            return true;
        }

        class CustomTimer : System.Timers.Timer
        {
            public WhiteboardConnection connection;
        }
        private async void CheckIsAlive(Object source, ElapsedEventArgs e)
        {
            WhiteboardConnection connection = ((CustomTimer)source).connection;

            byte[] data = new WhiteboardIsAliveMessage().Serialize();
            await SendMessageWithCheck(connection.Client, connection, (System.Timers.Timer)source, data);

            DateTime lastMessage = DateTime.MinValue;
            lastTimestamps.TryGetValue(connection, out lastMessage);
            if (DateTime.Now.Subtract(lastMessage).TotalMilliseconds > waitTime)
            {//No response from client

                whiteboards.TryGetValue(connection.RoomId, out Whiteboard whiteboard);
                whiteboard?.RemoveConnection(connection);
                await connection.Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "No response from client", CancellationToken.None);
                ((CustomTimer)source).Stop();
                ((CustomTimer)source).Dispose();
                return;
            }
        }

        private async Task ClientLoop(WebSocket client)
        {

            // Buffer for reading data
            Byte[] sentMessage = new Byte[1024 * 4];
            String data = null;
            WhiteboardMessage wMessage;
            int i;
            State connState = State.NewConnection;
            WhiteboardConnection wConn = new WhiteboardConnection(client);


            System.Timers.Timer isAliveTimer = null;


            WebSocketReceiveResult receiveResult = await client.ReceiveAsync(sentMessage, CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                // Translate data sentMessage to a ASCII string.
                //TODO take this out after testing
                data = System.Text.Encoding.ASCII.GetString(sentMessage, 0, receiveResult.Count);
                Console.WriteLine("Received: {0}", data);

                MessageType type = WhiteboardMessage.GetMessageType(sentMessage);
                wMessage = WhiteboardMessage.GetMessageFromType(type);
                wMessage = wMessage.DeSerialize(sentMessage);

                switch (connState)
                {
                    case State.NewConnection:
                        if (wMessage.Type != MessageType.Authentication || !AuthenticateMessage((WhiteboardAuthenticationMessage)wMessage))
                        {
                            //if it's not a successful authentication message
                            byte[] wbm = new WhiteboardDeniedMessage().Serialize();
                            await SendMessageWithCheck(client, wConn, isAliveTimer, wbm);
                        }
                        else
                        {
                            //if it is a successful authentication message
                            WhiteboardAuthenticationMessage auth = (WhiteboardAuthenticationMessage)wMessage;
                            whiteboards.TryAdd(auth.ChatroomId, new Whiteboard(auth.ChatroomId));
                            Whiteboard board;
                            whiteboards.TryGetValue(auth.ChatroomId, out board);
                            wConn = new WhiteboardConnection(auth.Username, auth.ChatroomId, client);
                            board?.AddConnection(wConn);
                            byte[] wbm = new WhiteboardOKMessage().Serialize();
                            await SendMessageWithCheck(client, wConn, isAliveTimer, wbm);
                            connState = State.Authenticated;


                            isAliveTimer = new CustomTimer
                            {
                                Interval = waitTime,
                                connection = wConn
                            };

                            isAliveTimer.Elapsed += CheckIsAlive;
                            isAliveTimer.AutoReset = true;
                            isAliveTimer.Enabled = true;
                            isAliveTimer.Start();
                            wConn.IsAliveTimer = isAliveTimer;
                            
                        }
                        break;
                    case State.Authenticated:
                        if (wMessage.Type != MessageType.Event)
                        {
                            if (wMessage.Type == MessageType.OK)
                            {
                                lastTimestamps.AddOrUpdate(wConn, DateTime.Now, (key, oldValue) => DateTime.Now);
                            }
                            else
                            {
                                //incorrect message type
                                byte[] wbm = new WhiteboardDeniedMessage().Serialize();
                                await SendMessageWithCheck(client, wConn, isAliveTimer, wbm);
                            }
                        }
                        else
                        {
                            //TODO check if casting works
                            WhiteboardEventMessage evMessage = (WhiteboardEventMessage)wMessage;
                            Whiteboard board;
                            whiteboards.TryGetValue(wConn.RoomId, out board);
                            //sending changes to whiteboard of this message
                            board?.AddEvents(evMessage.GetEvents(),this);
                        }
                        break;
                    default:
                        break;
                }
                receiveResult = await client.ReceiveAsync(sentMessage, CancellationToken.None);
            }
            if (wConn != null)
            {
                whiteboards.TryGetValue(wConn.RoomId, out Whiteboard whiteboard);
                whiteboard?.RemoveConnection(wConn);
            }
            isAliveTimer?.Stop();
            isAliveTimer?.Dispose();
        }

        public async Task<bool> SendMessageWithCheck(WebSocket client, WhiteboardConnection wConn, System.Timers.Timer? isAliveTimer, byte[] wbm)
        {
            try
            {
                await client.SendAsync(wbm, WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                await wConn.Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "No response from client", CancellationToken.None);
                whiteboards.TryGetValue(wConn.RoomId, out Whiteboard whiteboard);
                whiteboard?.RemoveConnection(wConn);
                isAliveTimer?.Stop();
                isAliveTimer?.Dispose();
            }
            return false;
        }

        public byte[] GetWhiteboardData(int chatroom)
        {
            whiteboards.TryGetValue(chatroom, out Whiteboard whiteboard);
            return whiteboard?.GetData();
        }

        public static void JsonTest()
        {
            WhiteboardOKMessage wbokm = new WhiteboardOKMessage();
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(wbokm.Serialize()));
            WhiteboardIsAliveMessage wbiam = new WhiteboardIsAliveMessage();
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(wbiam.Serialize()));
            WhiteboardDeniedMessage wbdm = new WhiteboardDeniedMessage();
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(wbdm.Serialize()));
            WhiteboardEventMessage wbem = new WhiteboardEventMessage();
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(wbem.Serialize()));
            WhiteboardAuthenticationMessage wbaum = new WhiteboardAuthenticationMessage();
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(wbaum.Serialize()));
        }
        public static void JsonMessageEventTest()
        {
            WhiteboardLineEvent wbLine = new WhiteboardLineEvent(new SKPoint(1,1), new SKPoint(20, 20), 0xFF000000);
            LinkedList<WhiteboardEvent> wbEvents = new LinkedList<WhiteboardEvent>();
            wbEvents.AddLast(wbLine);
            WhiteboardEventMessage wbem = new WhiteboardEventMessage(10, wbEvents);
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(wbem.Serialize()));
        }
    }
}
