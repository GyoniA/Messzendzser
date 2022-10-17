namespace Messzendzser.WhiteBoard
{
    using Messzendzser.Model.DB.Models;
    using Microsoft.Extensions.Logging;
    using Org.BouncyCastle.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
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

        //TODO szálbiztos lista kapcsolatokra ellenőrzése
        //stores each chatrooms whiteboard
        private ConcurrentDictionary<Chatroom, Whiteboard> whiteboards;


        private ConcurrentDictionary<WhiteboardConnection, DateTime> lastTimestamps;

        private CancellationTokenSource stop = new CancellationTokenSource();

        public async Task AcceptConnection(WebSocket socket)
        {
            await ClientLoop(socket);
        }

        public WhiteboardManager()
        {
            whiteboards = new ConcurrentDictionary<Chatroom, Whiteboard>();
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
            DateTime lastMessage = DateTime.MinValue;
            lastTimestamps.TryGetValue(connection, out lastMessage);
            if (DateTime.Now.Subtract(lastMessage).TotalMilliseconds > waitTime)
            {//No response from client
                whiteboards.TryGetValue(connection.Room, out Whiteboard whiteboard);
                whiteboard?.RemoveConnection(connection);
                ((CustomTimer)source).Stop();
                return;
            }
            byte[] data = new WhiteboardIsAliveMessage(new byte[0]).Serialize();

            await SendMessageWithCheck(connection.Client, connection, (System.Timers.Timer)source, data);
        }

        private async Task ClientLoop(WebSocket client)
        {

            // Buffer for reading data
            Byte[] sentMessage = new Byte[1024 * 4];
            String data = null;
            WhiteboardMessage wMessage;
            int i;
            State connState = State.NewConnection;
            WhiteboardConnection wConn = null;


            System.Timers.Timer isAliveTimer = null;


            WebSocketReceiveResult receiveResult = await client.ReceiveAsync(sentMessage, CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                // Translate data sentMessage to a ASCII string.
                //TODO take this out after testing
                data = System.Text.Encoding.ASCII.GetString(sentMessage, 0, receiveResult.Count);
                Console.WriteLine("Received: {0}", data);

                wMessage = new WhiteboardMessage(sentMessage);

                switch (connState)
                {
                    case State.NewConnection:
                        if (wMessage.MessageType != MessageType.Authentication || AuthenticateMessage((WhiteboardAuthenticationMessage)wMessage))
                        {
                            //if it's not a successful authentication message
                            byte[] wbm = new WhiteboardDeniedMessage(new byte[0]).Serialize();
                            await SendMessageWithCheck(client, wConn, isAliveTimer, wbm);
                        }
                        else
                        {
                            //if it is a successful authentication message
                            WhiteboardAuthenticationMessage auth = (WhiteboardAuthenticationMessage)wMessage;
                            bool authenticated = true;
                            if (authenticated)
                            {
                                whiteboards.TryAdd(auth.Chatroom, new Whiteboard(auth.Chatroom));
                                Whiteboard board;
                                whiteboards.TryGetValue(auth.Chatroom, out board);
                                wConn = new WhiteboardConnection(auth.Username, auth.Chatroom, client);
                                board?.AddConnection(wConn);
                                byte[] wbm = new WhiteboardOKMessage(new byte[0]).Serialize();
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
                        }
                        break;
                    case State.Authenticated:
                        if (wMessage.MessageType != MessageType.Event)
                        {
                            if (wMessage.MessageType == MessageType.OK)
                            {
                                lastTimestamps.AddOrUpdate(wConn, DateTime.Now, (key, oldValue) => DateTime.Now);
                            }
                            else
                            {
                                //incorrect message type
                                byte[] wbm = new WhiteboardDeniedMessage(new byte[0]).Serialize();
                                await SendMessageWithCheck(client, wConn, isAliveTimer, wbm);
                            }
                        }
                        else
                        {
                            WhiteboardEventMessage evMessage = (WhiteboardEventMessage)wMessage;
                            Whiteboard board;
                            whiteboards.TryGetValue(evMessage.Chatroom, out board);
                            //sending changes to whiteboard of this message
                            board?.AddEvents(evMessage.GetEvents());
                        }
                        break;
                    default:
                        break;
                }
            }
            whiteboards.TryGetValue(wConn.Room, out Whiteboard whiteboard);
            whiteboard?.RemoveConnection(wConn);
            isAliveTimer?.Stop();
            isAliveTimer?.Dispose();
        }

        public async Task<bool> SendMessageWithCheck(WebSocket client, WhiteboardConnection wConn, System.Timers.Timer isAliveTimer, byte[] wbm)
        {
            try
            {
                await client.SendAsync(wbm, WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                whiteboards.TryGetValue(wConn.Room, out Whiteboard whiteboard);
                whiteboard?.RemoveConnection(wConn);
                isAliveTimer?.Stop();
                isAliveTimer?.Dispose();
            }
            return false;
        }

        public byte[] GetWhiteboardData()
        {
            throw new NotImplementedException();
            //return this.whiteboard.GetData();
        }

        public byte[] GetWhiteboardData(Chatroom chatroom)
        {
            whiteboards.TryGetValue(chatroom, out Whiteboard whiteboard);
            return whiteboard?.GetData();
        }

        public void UpdateWhiteboard(byte[] message)
        {
            throw new NotImplementedException();
            //this.whiteboard.AddMessage(new WhiteboardMessage(sentMessage));
        }
        public void UpdateWhiteboard(Chatroom chatroom, LinkedList<WhiteboardEvent> newEvents)
        {
            whiteboards.TryGetValue(chatroom, out Whiteboard whiteboard);
            whiteboard?.AddEvents(newEvents);
        }
    }
}
