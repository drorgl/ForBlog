using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ChatMessages;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Configuration;

namespace ChatClient
{
    /// <summary>
    /// Chat Client Program
    /// </summary>
    class Program
    {
        /// <summary>
        /// Remote Certificate validation override method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Validate the certificate hash, if we're using a self signed certificate, at least check its the same one we issued.
            if (certificate.GetCertHashString() == "48D5760D449114D08E0297AD9BDB29028326BD96")
                return true;
            return false;
        }

        /// <summary>
        /// Configured Buffer Size
        /// </summary>
        private static readonly int ConfigBufferSize = ((int?)ConfigurationManagerEx.AppSettings["BufferSize"]).GetValueOrDefault(0);

        static void Main(string[] args)
        {
            //Sleep for 1 second so the Server had enough time starting
            Thread.Sleep(1000);

            TcpClient client = new TcpClient();

            //Initialize a memorybuffer for the client
            MemoryBuffer clientbuffer = new MemoryBuffer(ConfigBufferSize);

            //Connect to the server
            client.Connect(IPAddress.Parse("127.0.0.1"), 1000);
            Console.WriteLine("Connected to server");

            //Get client stream
            Stream clientstream = client.GetStream();

            //apply encryption if configured
            if (((bool?)ConfigurationManagerEx.AppSettings["Encryption"]).GetValueOrDefault(false) == true)
            {
                clientstream = new SslStream(clientstream, true, RemoteCertificateValidation, null);
                //authenticate the client as client in the sslstream
                ((SslStream)clientstream).AuthenticateAsClient("sslstreamtest", null, System.Security.Authentication.SslProtocols.Ssl3, false);
            }

            Common.DisplayStreamInformation(clientstream);

            //buffer the stream with async read, this is to simplify the understanding of program flow, usually should be done with Async methods
            clientstream = new StreamReadAsync(clientstream);

            //Apply compression if requested
            if (((bool?)ConfigurationManagerEx.AppSettings["Compression"]).GetValueOrDefault(false) == true)
            {
                clientstream = new CompressedNetworkStream(clientstream);
            }



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
                            Common.SendChatMessage(client, clientstream, "ping\r");
                        }

                    }

                    //send the message, zero it and unflag the sendmessage
                    Common.SendChatMessage(client, clientstream, message);
                    message = "";
                    sendmessage = false;

                    //flush the stream buffers.
                    clientstream.Flush();
                }

                //read data from client and copy to clientbuffer
                if (client.Connected)
                {
                    clientstream.CopyTo(clientbuffer);
                }

                //check if a message arrived

                do
                {

                    ChatMessage chatmessage = null;
                    //read one whole message from the buffer
                    Common.ReadStreamToMessageStream(clientbuffer, out chatmessage);

                    if (chatmessage != null)
                    {
                        //if we're inside multiping, do not display messages
                        if (multipingsent == false)
                            Console.WriteLine("{0} Received: {1}", DateTime.Now, chatmessage.Message);

                        //if the message received is ping, send a pong
                        if (chatmessage.Message.Equals("ping\r", StringComparison.InvariantCultureIgnoreCase))
                            Common.SendChatMessage(client, clientstream, "pong\r");

                        //if the message received is a pong, add it to the multiping counter, if the requests equals the
                        //response iterations, show how much time it took to receive these messages
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
                    while (clientbuffer.Length > 0);

                //flush the buffers (for the replying part).
                clientstream.Flush();

            }



        }
    }
}

