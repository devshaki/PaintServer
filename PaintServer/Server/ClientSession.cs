using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using PaintServer.FileSystem;

namespace PaintServer.Server
{
    class ClientSession
    {
        public TcpClient client;
        private CancellationTokenSource cancellationTokenSource;
        public string clientId { get; private set; }
        private FileManager fileManager;
        


        public ClientSession(TcpClient client)
        {
            this.client = client;
            this.clientId = Guid.NewGuid().ToString();
            fileManager = FileManager.getFileManager();
        }

        public async Task Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            await ReceiveMessages(cancellationTokenSource);
        }

        public async Task Stop()
        {
            client.Close();
            cancellationTokenSource.Cancel();
        }

        public async Task ReceiveMessages(CancellationTokenSource cancellationTokenSource)
        {
            NetworkStream ns = client.GetStream();

            while (client.Connected && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                byte[] message = new byte[100];
                int bytesRead = await ns.ReadAsync(message, 0, message.Length);
                string messageString = Encoding.UTF8.GetString(message, 0, bytesRead);
                
                if (messageString.StartsWith("upload:"))
                {
                    String[] metaData = messageString.Split(':');
                    String filename = metaData[1];
                    int filesize = int.Parse(metaData[2]);
                    await ReceiveFile(filesize, filename, cancellationTokenSource);


                }
                else if (messageString.StartsWith("get:"))
                {
                    string fileName = messageString.Split(':')[1];
                    await SendFile(fileName, clientId);
                }

                else if (messageString.StartsWith("close:"))
                {
                    string fileName = messageString.Split(':')[1];
                    fileManager.closeFile(fileName, clientId);
                }

            }

        }

        public async Task SendFile(string fileName,string clientId)
        {
            string jsonData = fileManager.openFile(fileName, clientId);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
            int filesize = jsonBytes.Length;

            NetworkStream ns = client.GetStream();

            string header = $"success:{fileName}:{filesize}";
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);

            await ns.WriteAsync(headerBytes, 0, headerBytes.Length);

            await Task.Delay(100);

            await ns.WriteAsync(jsonBytes, 0, filesize);
        }
        public async Task ReceiveFile(int fileSize, string filename,CancellationTokenSource cancellationTokenSource)
        {
            NetworkStream ns = client.GetStream();
            while (client.Connected && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                byte[] fileData= new byte[fileSize];
                int totalRead = 0;

                while (totalRead < fileSize)
                {
                    int bytesRead = await ns.ReadAsync(fileData, totalRead, fileSize - totalRead);
                    totalRead += bytesRead;
                }

                string jsonData = Encoding.UTF8.GetString(fileData);
                fileManager.saveFile(filename, jsonData, clientId);

            }

        }


    }
}
