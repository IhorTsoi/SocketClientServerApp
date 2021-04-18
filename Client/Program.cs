using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        private static readonly int defaultPort = 7777;
        private static readonly IPAddress defaultIpAddress = IPAddress.Parse("127.0.0.1");
        private static readonly string defaultEndOfMessageSymbol = "\n\n";

        static void Main()
        {
            // the task uses unicode characters
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var clientSocket = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clientSocket.Connect(defaultIpAddress, defaultPort);
            Console.WriteLine($"Connected to server [{defaultIpAddress}:{defaultPort}]");

            Console.WriteLine("Please type the words to be sent. Use the \" \" as a delimiter.");
            var requestMessage = Console.ReadLine();
            SendMessage(clientSocket, requestMessage);
            Console.WriteLine("Message was sent: " + requestMessage);

            var responseMessage = ReceiveMessage(clientSocket);
            Console.WriteLine("Response received: " + responseMessage);

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private static void SendMessage(Socket clientSocket, string requestMessage)
        {
            var bytesToBeSent = Encoding.Unicode.GetBytes(requestMessage + defaultEndOfMessageSymbol);
            clientSocket.Send(bytesToBeSent);
        }

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
