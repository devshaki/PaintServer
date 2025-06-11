using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using PaintClient;
namespace PaintClient.networking
{
    class NetworkClient
    {
        TcpClient client;
        private NetworkStream stream;
        private int port;
        private string ipAdress;
        private int maxHeaderSize = 100;
        public static Action<string> RecivedFile;
        public NetworkClient(string ipAdress,int port)
        {
            this.port = port;
            this.ipAdress = ipAdress;
        }

        public async Task Connect()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(ipAdress, port);
                stream = client.GetStream();
                await ReceiveMessages();
            }
            catch
            {
                Console.WriteLine("failed to connect to the server");
            }
        }

        public async Task SendHeader(string header,string filename,int size,string jsonData)
        {
            string fullheader = $"{header}:{filename}:{size}";
            if (stream == null || client == null) 
            {
                Console.WriteLine("stream or client is null");
                return; 
            }
            try
            {
                byte[] headerBytes = Encoding.UTF8.GetBytes(fullheader);

                await stream.WriteAsync(headerBytes, 0, headerBytes.Length);

                await SendJson(jsonData);
            }
            catch
            {
                Console.WriteLine("failed to send header");
            }

        }
        public async Task SendHeader(string header, string filename)
        {
            string fullheader = $"{header}:{filename}";
            Console.WriteLine($"sending header {fullheader}");
            try
            {
                byte[] headerBytes = Encoding.UTF8.GetBytes(fullheader);

                await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
            }
            catch
            {
                Console.WriteLine("failed to send header");
            }
        }
        public async Task SendJson(string jsonData)
        {
            Console.WriteLine($"sending {jsonData}");
            try
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

                await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
            }
            catch
            {
                Console.WriteLine("failed to send json");
            }

        }

        public async Task ReceiveMessages()
        {
            NetworkStream ns = client.GetStream();
            Console.WriteLine("ReceiveMessages");

            while (client.Connected)
            {
                byte[] message = new byte[maxHeaderSize];
                int bytesRead = await ns.ReadAsync(message, 0, message.Length);
                try
                {
                    string messageString = Encoding.UTF8.GetString(message, 0, bytesRead);
                    String[] metaData = messageString.Split(':');
                    Console.WriteLine(messageString);
                    switch (metaData[0])
                    {
                        case "success":
                            {

                                Console.WriteLine("got success");
                                String filename = metaData[1];
                                int filesize = int.Parse(metaData[2]);
                                await ReceiveFile(filesize);
                                break;
                            }
                    }
                }
                catch
                {
                    Console.WriteLine($"message decode failed");
                }


            }

        }


        public async Task ReceiveFile(int fileSize)
        {
            NetworkStream ns = client.GetStream();
            byte[] fileData = new byte[fileSize];
            int totalRead = 0;

            while (totalRead < fileSize)
            {
                int bytesRead = await ns.ReadAsync(fileData, totalRead, fileSize - totalRead);
                totalRead += bytesRead;
            }
            try
            {
                string jsonData = Encoding.UTF8.GetString(fileData);
                RecivedFile(jsonData);
            }
            catch
            {
                Console.WriteLine($"message decode failed");
            }



        }
    }
}
