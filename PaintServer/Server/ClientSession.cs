using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace PaintServer.Server
{
    class ClientSession
    {
        public TcpClient client;
        private CancellationTokenSource cancellationTokenSource;

        public ClientSession(TcpClient client)
        {
            this.client = client;
        }

        public async Task Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            await ReceiveMessages(cancellationTokenSource);
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


                }
                else if (messageString.StartsWith("get:"))
                {
                    string fileName = messageString.Split(':')[1];
                }

            }

        }

        public async Task ReceiveFile(int fileSize, string name,CancellationTokenSource cancellationTokenSource)
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
            }

        }


    }
}
