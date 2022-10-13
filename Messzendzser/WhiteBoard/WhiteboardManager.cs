﻿namespace Messzendzser.WhiteBoard
{
    using Messzendzser.Model.DB.Models;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class WhiteboardManager : IWhiteboardManager
    {
        private Whiteboard whiteboard;
        private LinkedList<WhiteboardConnection> clients = new LinkedList<WhiteboardConnection>();
        private CancellationTokenSource stop = new CancellationTokenSource();

        public WhiteboardManager(Chatroom cr)
        {
            this.whiteboard = new Whiteboard(cr);
            
            TcpListener server = null;
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
                ListeningLoop(server).Wait();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
        // szálbiztos lista kapcsolatokra
        private async Task ListeningLoop(TcpListener server)
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
                        // Translate data bytes to a ASCII string.
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
            return this.whiteboard.GetData();
        }

        public void UpdateWhiteboard(byte[] message)
        {
            this.whiteboard.AddMessage(new WhiteboardMessage(message));
        }
    }
}
