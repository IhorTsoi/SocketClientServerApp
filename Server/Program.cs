using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        private static readonly int defaultPort = 7777;
        private static readonly IPAddress defaultIpAddress = IPAddress.Any;
        private static readonly string defaultEndOfMessageSymbol = "\n\n";

        static void Main()
        {
            // the task uses unicode characters
            Console.OutputEncoding = Encoding.Unicode;

            var listeningSocket = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listeningSocket.Bind(new IPEndPoint(defaultIpAddress, defaultPort));
            listeningSocket.Listen(backlog: 1);
            Console.WriteLine($"Listening on any IP address, port: {defaultPort}");


            var handler = listeningSocket.Accept();
            Console.WriteLine("Connection was accepted.");

            var requestMessage = ReceiveMessage(handler);
            Console.WriteLine("Request message was received: " + requestMessage);

            var wordsToBeSentBack = PerformTask(requestMessage);
            var responseMessage = string.Join(", ", wordsToBeSentBack);

            SendMessage(handler, responseMessage);
            Console.WriteLine("Response message was send: " + responseMessage);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void SendMessage(Socket handler, string message)
        {
            byte[] bytesToBeSend = Encoding.Unicode.GetBytes(message + defaultEndOfMessageSymbol);
            handler.Send(bytesToBeSend);
        }

        private static IEnumerable<string> PerformTask(string message) =>
            message.Split(' ').Where(word => word.ToLower().EndsWith("во"));

        private static string ReceiveMessage(Socket handler)
        {
            var receiveBuffer = new byte[1024];
            var receivedMessageParts = new StringBuilder();

            while (true)
            {
                int bytesReceivedCount = handler.Receive(receiveBuffer);
                string messagePart = Encoding.Unicode.GetString(
                    receiveBuffer, 0, bytesReceivedCount);
                receivedMessageParts.Append(messagePart);

                if (messagePart.Contains(defaultEndOfMessageSymbol))
                {
                    break;
                }
            }

            return receivedMessageParts.ToString()
                .TrimEnd(defaultEndOfMessageSymbol.ToCharArray());
        }
    }
}
