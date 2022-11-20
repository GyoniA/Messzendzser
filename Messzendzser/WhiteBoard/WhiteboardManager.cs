namespace Messzendzser.WhiteBoard
{
    using Messzendzser.Model.DB;
    using Messzendzser.Model.DB.Models;
    using Messzendzser.Utils;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
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

        private const int bufferSize = 1024 * 40;
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
        public bool AuthenticateMessage(WhiteboardAuthenticationMessage wm)
        {
            //return true;//TODO remove

            var res = false;
            UserToken token = new UserToken(wm.Username);
            User user = token.ToUser();
            try
            {
                using (IDataSource dataSource = new MySQLDbConnection())
                {
                    res = dataSource.IsUserInChatroom(user.Id, wm.ChatroomId);
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return res;
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
            if (DateTime.Now.Subtract(lastMessage).TotalMilliseconds > waitTime*2)
            {//No response from client

                whiteboards.TryGetValue(connection.RoomId, out Whiteboard whiteboard);
                whiteboard?.RemoveConnection(connection);
                if(connection.Client.State!=WebSocketState.Closed)
                    await connection.Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "No response from client", CancellationToken.None);
                ((CustomTimer)source).Stop();
                ((CustomTimer)source).Dispose();
                return;
            }
        }

        private async Task ClientLoop(WebSocket client)
        {

            // Buffer for reading data
            Byte[] sentMessage = new Byte[bufferSize];
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
                
                wMessage = WhiteboardMessage.DeSerialize(sentMessage);

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
                            Whiteboard board;
                            if(!whiteboards.TryGetValue(auth.ChatroomId, out board))
                            {
                                whiteboards.TryAdd(auth.ChatroomId, new Whiteboard(auth.ChatroomId));

                                board?.LoadImageFromFile();
                            }
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
                            lastTimestamps.AddOrUpdate(wConn, DateTime.Now, (key, oldValue) => DateTime.Now);
                            byte[] wbiam = new WhiteboardIsAliveMessage().Serialize();
                            await SendMessageWithCheck(client, wConn, isAliveTimer, wbiam);
                            isAliveTimer.Elapsed += CheckIsAlive;
                            isAliveTimer.AutoReset = true;
                            isAliveTimer.Enabled = true;
                            isAliveTimer.Start();
                            wConn.IsAliveTimer = isAliveTimer;
                            
                            board?.SaveDataToFile();
                            WhiteboardEventMessage wbim = new WhiteboardEventMessage();
                            wbim.AddEvent(new WhiteboardImageEvent(wConn.RoomId));
                            byte[] imageMessage = wbim.Serialize();
                            await SendMessageWithCheck(client, wConn, isAliveTimer, imageMessage);
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
                            WhiteboardEventMessage evMessage = (WhiteboardEventMessage)wMessage;
                            Whiteboard board;
                            whiteboards.TryGetValue(wConn.RoomId, out board);
                            //sending changes to whiteboard of this message
                            
                            board?.AddEvents(evMessage.GetEvents(),this, wConn);
                        }
                        break;
                    default:
                        break;
                }
                sentMessage = new byte[bufferSize];
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
                if (wConn.Client.State != WebSocketState.Closed)
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
            Console.WriteLine("\nend of jsontest\n\n");
        }
        public static void JsonMessageEventTest()
        {
            WhiteboardLineEvent wbLine = new WhiteboardLineEvent(new Point(1,1), new Point(20, 20), 0xFF000000);
            LinkedList<WhiteboardEvent> wbEvents = new LinkedList<WhiteboardEvent>();
            wbEvents.AddLast(wbLine);
            WhiteboardEventMessage wbem = new WhiteboardEventMessage(10, wbEvents);
            Console.Write("Event message with a line event: ");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(wbem.Serialize()));
            /*// clutters console so only use if testing
            WhiteboardImageEvent wbImage = new WhiteboardImageEvent(10);
            wbEvents.AddLast(wbImage);
            wbem = new WhiteboardEventMessage(10, wbEvents);
            Console.Write("Event message with a line and image event: ");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(wbem.Serialize()));*/

            Console.WriteLine("\nend of json message event test\n\n");
        }
    }
}
