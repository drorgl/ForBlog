using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;

namespace ChatMessages
{
    /// <summary>
    /// Chat common methods
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Serializes and prepares the packet stream needed to transfer a message
        /// </summary>
        public static MemoryStream GetStreamFromMessage(object message)
        {
            byte[] messagebytes = Serialize(message);
            int length = messagebytes.Length;
            byte[] lengthbytes = BitConverter.GetBytes(length);
            MemoryStream messagestream = new MemoryStream();
            messagestream.Write(lengthbytes, 0, lengthbytes.Length);
            messagestream.Write(messagebytes, 0, messagebytes.Length);
            return messagestream;
        }

        /// <summary>
        /// Serializes an object to byte array
        /// </summary>
        public static byte[] Serialize(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes an object from MemoryStream
        /// </summary>
        public static object Deserialize(MemoryStream ms)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(ms);
        }

        /// <summary>
        /// Reads a key from the console
        /// </summary>
        public static void ReadMessage(ref bool sendmessage, ref string message)
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                message += key.KeyChar;
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    sendmessage = true;
                }
            }
        }

        /// <summary>
        /// Attempts to read a whole message from fromstream
        /// </summary>
        /// <param name="fromstream"></param>
        /// <returns></returns>
        public static void ReadStreamToMessageStream(MemoryBuffer fromstream, out ChatMessage message)
        {
            message = null;

            //check that we have at least 4 bytes in the buffer, it contains the length of the message
            if (fromstream.Length > 4)
            {
                byte[] lenbuf = new byte[4];
                var peeklen = fromstream.Peek(lenbuf, 0, lenbuf.Length);
                if (peeklen == 4)
                {
                    //check that we have the whole message in the buffer
                    var messagelength = BitConverter.ToInt32(lenbuf, 0);
                    if (fromstream.Length >= messagelength + 4)
                    {
                        var messagebuffer = new byte[messagelength];

                        //clear the length from the buffer
                        fromstream.Read(lenbuf, 0, lenbuf.Length);

                        //read the message from the buffer
                        fromstream.Read(messagebuffer, 0, messagelength);

                        var messagestream = new MemoryStream(messagebuffer, 0, messagelength);
                        message = DecodeMessage(messagestream);


                    }
                }
            }
        }

        /// <summary>
        /// Decodes (deserializes) a message from the memorystream
        /// </summary>
        private static ChatMessage DecodeMessage(MemoryStream messagestream)
        {
            ChatMessage chatmessage = (ChatMessage)Common.Deserialize(messagestream);
            return chatmessage;
        }

        /// <summary>
        /// Attempt to send a message to the client if its still connected.
        /// </summary>
        public static void SendChatMessage(TcpClient client, Stream clientStream, string message)
        {
            ChatMessage chatmessage = new ChatMessage { Message = message };
            var messagestream = Common.GetStreamFromMessage(chatmessage);
            if (client.Connected)
            {
                var clientstream = clientStream;
                byte[] messagebuffer = messagestream.ToArray();
                clientstream.Write(messagebuffer, 0, messagebuffer.Length);
            }
        }


        #region SslStream information

        /// <summary>
        /// Displays Stream information
        /// </summary>
        /// <param name="stream"></param>
        public static void DisplayStreamInformation(Stream stream)
        {
            if (stream is SslStream)
            {
                DisplaySecurityLevel((SslStream)stream);
                DisplaySecurityServices((SslStream)stream);
                DisplayCertificateInformation((SslStream)stream);
            }
            DisplayStreamProperties(stream);
        }

        /// <summary>
        /// Displays security information about the SslStream
        /// </summary>
        /// <param name="stream"></param>
        private static void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine("Cipher: {0} strength {1} Hash: {2} strength {3}", stream.CipherAlgorithm, stream.CipherStrength, stream.HashAlgorithm, stream.HashStrength);
            Console.WriteLine("Key exchange: {0} strength {1} Protocol {2}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength, stream.SslProtocol);
        }

        /// <summary>
        /// Display security status of the SslStream
        /// </summary>
        /// <param name="stream"></param>
        private static void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine("Is authenticated: {0} as server? {1}, IsSigned: {2}, Is Encrypted: {3}", stream.IsAuthenticated, stream.IsServer, stream.IsSigned, stream.IsEncrypted);
        }

        /// <summary>
        /// Displays stream usability
        /// </summary>
        /// <param name="stream"></param>
        private static void DisplayStreamProperties(Stream stream)
        {
            Console.WriteLine("Can read: {0}, write {1}, Can timeout: {2}", stream.CanRead, stream.CanWrite, stream.CanTimeout);
        }

        /// <summary>
        /// Displays SslStream certificate information
        /// </summary>
        /// <param name="stream"></param>
        private static void DisplayCertificateInformation(SslStream stream)
        {
            Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Local certificate is null.");
            }
            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Remote certificate is null.");
            }
        }
        #endregion
    }
}
