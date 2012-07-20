using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;
using ChatMessages;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.Diagnostics;

namespace ChatServer
{
    /// <summary>
    /// Chat Server Program
    /// </summary>
    class Program
    {
        /// <summary>
        /// Container for each client connection
        /// </summary>
        public class ClientConnection
        {
            public TcpClient Client { get; set; }
            public MemoryBuffer Buffer { get; set; }
            public Stream Stream { get; set; }
        }

        /// <summary>
        /// Configured Buffer Size
        /// </summary>
        private static readonly int ConfigBufferSize = ((int?)ConfigurationManagerEx.AppSettings["BufferSize"]).GetValueOrDefault(0);

        static void Main(string[] args)
        {
            //Server Certificate
            X509Certificate serverCertificate = new X509Certificate("sslstreamtestcert.pfx","123456");
            
            //List for each connected client
            List<ClientConnection> clients = new List<ClientConnection>();

            //TCP Listener
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1000);

            //Start Listening
            listener.Start();
            Console.WriteLine("Started listening");

            //flag that specifies if the message variable should be sent
            bool sendmessage = false;

            //message variable, the program will add characters to this string until sendmessage is flagged.
            string message = "";

            //multiple ping iterations, the number of pings sent to the other side and the number of pongs to expect
            int multiping_iterations = 10000;

            //the number of pongs received while in multiping
            int multiping_iterations_received = 0;

            //a flag specifying if a multiping was sent, to avoid receiving messages while multiping is executing.
            bool multipingsent = false;

            Stopwatch multiping_stopwatch = new Stopwatch();

            while (true)
            {
                //if there's a pending connection, initialize it.
                if (listener.Pending())
                {
                    //Accept connection
                    var connection = new ClientConnection { Client = listener.AcceptTcpClient(), Buffer = new MemoryBuffer(ConfigBufferSize) };

                    Stream stream = connection.Client.GetStream();

                    //apply encryption if configured
                    if (((bool?)ConfigurationManagerEx.AppSettings["Encryption"]).GetValueOrDefault(false) == true)
                    {
                        stream = new SslStream(stream);
                        ((SslStream)stream).AuthenticateAsServer(serverCertificate, false, SslProtocols.Ssl3, false);
                    }

                    Common.DisplayStreamInformation(stream);

                    //buffer the stream with async read, this is to simplify the understanding of program flow, usually should be done with Async methods
                    stream = new StreamReadAsync(stream);

                    //Apply compression if requested
                    if (((bool?)ConfigurationManagerEx.AppSettings["Compression"]).GetValueOrDefault(false) == true)
                    {
                        stream = new CompressedNetworkStream(stream);
                    }

                    

                    connection.Stream = stream;


                    clients.Add(connection);
                    Console.WriteLine("Client connected");
                }

                //Read message from Console.
                Common.ReadMessage(ref sendmessage, ref message);

                //If sendmessage is flagged, send the message
                if (sendmessage == true)
                {
                    //if the sendmessage requested is a multiping, send the amount of pings to the other side.
                    if (message == "multiping\r")
                    {
                        multipingsent = true;
                        multiping_iterations_received = 0;
                        multiping_stopwatch.Restart();
                        for (int i = 0; i < multiping_iterations; i++)
                        {
                            foreach (var client in clients)
                                Common.SendChatMessage(client.Client, client.Stream, "ping\r");
                        }
                        
                    }

                    //send the message, zero it and unflag the sendmessage
                    foreach (var client in clients)
                        Common.SendChatMessage(client.Client,client.Stream, message);
                    message = "";
                    sendmessage = false;

                    //flush the stream buffers.
                    foreach (var client in clients)
                        client.Stream.Flush();
                }

                //read data from clients
                foreach (var client in clients)
                {
                    if (client.Client.Connected)
                    {
                        client.Stream.CopyTo(client.Buffer);
                    }
                }

                //check if full message arrived
                foreach (var client in clients)
                {
                    do
                    {
                        ChatMessage chatmessage = null;
                        //read one whole message from the buffer
                        Common.ReadStreamToMessageStream(client.Buffer, out chatmessage);

                        if (chatmessage != null)
                        {
                            //if we're inside multiping, do not display messages
                            if (multipingsent == false)
                                Console.WriteLine("{0} Received: {1}", DateTime.Now, chatmessage.Message);

                            //if the message received is ping, send a pong
                            if (chatmessage.Message.Equals("ping\r", StringComparison.InvariantCultureIgnoreCase))

                                Common.SendChatMessage(client.Client, client.Stream, "pong\r");

                            //if the message received is a pong, add it to the multiping counter, if the requests equals the
                            //response iterations, show how much time it took to receive these messages
                            //BUG: possible bug if multiple clients are connected
                            if (chatmessage.Message.Equals("pong\r", StringComparison.InvariantCultureIgnoreCase))
                            {
                                multiping_iterations_received++;
                                if (multiping_iterations == multiping_iterations_received)
                                {
                                    multipingsent = false;
                                    Console.WriteLine("Received {0} pongs in {1}ms.", multiping_iterations_received, multiping_stopwatch.ElapsedMilliseconds);
                                }
                            }

                        }
                    } //while we have messages in the buffer, continue showing messages
                        while (client.Buffer.Length > 0);

                    //flush the buffers (for the replying part).
                    client.Stream.Flush();
                }
            }
        }

    }
}
