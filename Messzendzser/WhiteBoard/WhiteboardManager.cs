namespace Messzendzser.WhiteBoard
{
    using Messzendzser.Model.DB.Models;
    using Org.BouncyCastle.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class WhiteboardManager : IWhiteboardManager
    {
        enum State
        {
            NewConnection,
            Authenticated
        }

        //TODO szálbiztos lista kapcsolatokra ellenőrzése
        //stores each chatrooms whiteboard
        private ConcurrentDictionary<Chatroom, Whiteboard> whiteboards;
        
        private CancellationTokenSource stop = new CancellationTokenSource();
        TcpListener server;
        
        public WhiteboardManager()
        {
            this.whiteboards = new ConcurrentDictionary<Chatroom, Whiteboard>();
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
                            //TODO send denied message
                            new WhiteboardDeniedMessage(new byte[0]).Serialize();
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
                                board?.AddConnection(new WhiteboardConnection(auth.Username, auth.Chatroom, client));
                                //TODO send OK message
                                new WhiteboardOKMessage(new byte[0]).Serialize();
                                connState = State.Authenticated;
                            }
                        }
                        break;
                    case State.Authenticated:
                        if (wMessage.MessageType != MessageType.Event)
                        {
                            //incorrect message type
                            //TODO send denied message
                            new WhiteboardDeniedMessage(new byte[0]).Serialize();
                        }
                        else
                        {
                            WhiteboardEventMessage evMessage = (WhiteboardEventMessage)wMessage;
                            Whiteboard board;
                            whiteboards.TryGetValue(evMessage.Chatroom, out board);
                            //sending changes to whiteboard of this message
                            board?.AddEvents(evMessage.GetEvents());
                        }
                        /** TODO
                    pár másodpercenként IsAlive küldése
                    ha hiba/nem kap választ: kapcsolat megszakadt, takarítás, ha nincs kapcsolat a chatroomhoz akkor azt ki kell írni fileba*/
                        break;
                    default:
                        break;
                }

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", data);
            }

            throw new NotImplementedException();
        }

        private void ListeningLoop(TcpListener server)
        {

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;
            _ = Task.Run(async () =>
            {
                while (!stop.IsCancellationRequested)
                {
                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = await server.AcceptTcpClientAsync();
                    //new Thread(client)...{
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

                    Console.WriteLine("Connected!");
                    //TODO  check authentication with state machine
                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data sentMessage to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            });
        }

        public byte[] GetWhiteboardData()
        {
            throw new NotImplementedException();
            //return this.whiteboard.GetData();
        }

        public void UpdateWhiteboard(byte[] message)
        {
            throw new NotImplementedException();
            //this.whiteboard.AddMessage(new WhiteboardMessage(sentMessage));
        }
    }
}
