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
        TcpListener server;
        
        public WhiteboardManager()
        {
            whiteboards = new ConcurrentDictionary<Chatroom, Whiteboard>();
            lastTimestamps = new ConcurrentDictionary<WhiteboardConnection, DateTime>();
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Enter the listening loop.
                ListeningLoop(server);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server?.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
        public static bool AuthenticateMessage(WhiteboardAuthenticationMessage wm)
        {
            throw new NotImplementedException();
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
        private void CheckIsAlive(Object source, ElapsedEventArgs e)
        {
            WhiteboardConnection connection = ((CustomTimer)source).connection;
            DateTime lastMessage = DateTime.MinValue;
            lastTimestamps.TryGetValue(connection, out lastMessage);
            if (DateTime.Now.Subtract(lastMessage).TotalMilliseconds > waitTime)
            {//No response from client
                connection.Client.Close();
                whiteboards.TryGetValue(connection.Room, out Whiteboard whiteboard);
                whiteboard?.RemoveConnection(connection);
                ((CustomTimer)source).Stop();
                return;
            }
                byte[] data = new WhiteboardIsAliveMessage(new byte[0]).Serialize();
                NetworkStream stream = connection.Client.GetStream();
                stream.Write(data, 0, data.Length);

            SendMessageWithCheck(connection.Client, stream, connection, (System.Timers.Timer)source, data);
        }

        private void ClientLoop(object? state)
        {
            TcpClient client = (TcpClient)state;
            NetworkStream stream = client.GetStream();

            // Buffer for reading data
            Byte[] sentMessage = new Byte[256];
            String data = null;
            WhiteboardMessage wMessage;
            int i;
            State connState = State.NewConnection;
            WhiteboardConnection wConn = null;


            System.Timers.Timer isAliveTimer = null;
            
            while ((i = stream.Read(sentMessage, 0, sentMessage.Length)) != 0)
            {
                // Translate data sentMessage to a ASCII string.
                //TODO take this out after testing
                data = System.Text.Encoding.ASCII.GetString(sentMessage, 0, i);
                Console.WriteLine("Received: {0}", data);

                wMessage = new WhiteboardMessage(sentMessage);
                
                switch (connState)
                {
                    case State.NewConnection:
                        if (wMessage.MessageType != MessageType.Authentication || AuthenticateMessage((WhiteboardAuthenticationMessage)wMessage))
                        {
                            //if it's not a successful authentication message
                            byte[] wbm = new WhiteboardDeniedMessage(new byte[0]).Serialize();
                            SendMessageWithCheck(client, stream, wConn, isAliveTimer, wbm);
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
                                wConn =  new WhiteboardConnection(auth.Username, auth.Chatroom, client);
                                board?.AddConnection(wConn);
                                byte[] wbm = new WhiteboardOKMessage(new byte[0]).Serialize();
                                SendMessageWithCheck(client, stream, wConn, isAliveTimer, wbm);
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
                                SendMessageWithCheck(client, stream, wConn, isAliveTimer, wbm);
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
            client?.Close();
            whiteboards.TryGetValue(wConn.Room, out Whiteboard whiteboard);
            whiteboard?.RemoveConnection(wConn);
            isAliveTimer?.Stop();
            isAliveTimer?.Dispose();
        }

        private bool SendMessageWithCheck(TcpClient client, NetworkStream stream, WhiteboardConnection wConn, System.Timers.Timer isAliveTimer, byte[] wbm)
        {
            try
            {
                stream.Write(wbm, 0, wbm.Length);
                return true;
            }
            catch (Exception)
            {
                client?.Close();
                whiteboards.TryGetValue(wConn.Room, out Whiteboard whiteboard);
                whiteboard?.RemoveConnection(wConn);
                isAliveTimer?.Stop();
                isAliveTimer?.Dispose();
                //TODO CHECK IF CORRECT
                throw;
            }
            return false;
        }

        private void ListeningLoop(TcpListener server)
        {
            _ = Task.Run(async () =>
            {
                while (!stop.IsCancellationRequested)
                {
                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = await server.AcceptTcpClientAsync();
                    ThreadPool.QueueUserWorkItem(ClientLoop, client);
                    /*new Thread(client)...{
                     * NetworkStream stream = client.GetStream();
                     * while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)...
                     * byte[] message
                       state machine(new connection, authenticated)
                        switch(state){
                            newConnection:
                                message feldoozása (legyen authenticate typusú) mint WhiteboardAuthenticationMessage
                                ha elfogadjuk Ok küldése és authenticated állapotba rakni ha nem Denied küldése
                                WhiteboardImageEvent küldése, hol tart éppen a rajzolgatás
                                kapcsolat felvétele new WhiteboadConnection(chatrommid,client...)
                            authenticated:
                                message feldolgozása amúgy
                                új event message érkezik:
                                    kikeressük az összes chatroomhoz tartozó WhiteboadConnection, összesnek elküldeni, hogy mi változott
                            pár másodpercenként IsAlive küldése
                            ha hiba/nem kap választ: kapcsolat megszakadt, takarítás, ha nincs kapcsolat a chatroomhoz akkor azt ki kell írni fileba
                                    
                        }
                    }*/
                }
            });
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
