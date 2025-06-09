using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using PaintServer.Server;

namespace PaintServer.Server
{
    class Server
    {
        private readonly int port;
        private readonly string dbDirectory;
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource;
        private ClientManager clientManager;

        public bool IsRunning { get; private set; }

        public Server(int port,string dbDirectory)
        {
            this.port = port;
            this.dbDirectory = dbDirectory;
        }

        public async Task Start()
        {
            if (IsRunning) { return; }

            clientManager = ClientManager.GetClientManager();
            cancellationTokenSource = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any,port);
            listener.Start();

            IsRunning = true;

            Console.WriteLine($"server is now running on port {port}");
        }

        public async Task Suspend()
        {
            if (!IsRunning) { return; }

            cancellationTokenSource.Cancel();
            listener.Stop();

            Console.WriteLine("server is now suspended");
        }

        public async Task AcceptClients()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                await clientManager.AddClient(client);

            }
        }
    }
}
