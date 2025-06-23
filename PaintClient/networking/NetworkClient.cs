using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using PaintClient;
using System.ComponentModel;
namespace PaintClient.networking
{
    class NetworkClient
    {
        public static NetworkClient networkClient;
        public TcpClient client;
        private NetworkStream stream;
        private int port;
        private string ipAdress;
        private readonly int maxHeaderSize = 100;
        public static Action<string> RecivedFile;
        public static Action<string> RecivedFiles;
        private NetworkClient(string ipAdress,int port)
        {
            this.port = port;
            this.ipAdress = ipAdress;
        }

        public static NetworkClient GetNetworkClient(string ipAdress, int port)
        {
            if(networkClient == null)
            {
                networkClient = new NetworkClient(ipAdress, port);
            }
            return networkClient;
        }

        public async Task Connect()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(ipAdress, port);
                stream = client.GetStream();
                System.Diagnostics.Debug.WriteLine("connected to server");
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
            System.Diagnostics.Debug.WriteLine($"sending header {fullheader}");
            if (stream == null) { return; }
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
                    switch (metaData[0])
                    {
                        case "success":
                            {

                                System.Diagnostics.Debug.WriteLine("got success");
                                String filename = metaData[1];
                                int filesize = int.Parse(metaData[2]);
                                await ReceiveFile(filesize,filename);
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


        public async Task ReceiveFile(int fileSize,string fileName)
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
                System.Diagnostics.Debug.WriteLine(jsonData);
                System.Diagnostics.Debug.WriteLine("----------------");
                System.Diagnostics.Debug.WriteLine(fileName);
                if(fileName == "all")
                {
                    RecivedFiles(jsonData);
                }
                else
                {
                    RecivedFile(jsonData);
                }

            }
            catch
            {
                Console.WriteLine($"message decode failed");
            }



        }
    }
}
