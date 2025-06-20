﻿using System;
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
    class FileServer
    {
        private readonly int port;
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource;
        private ClientManager clientManager;

        public bool IsRunning { get; private set; }

        public FileServer(int port)
        {
            this.port = port;
        }

        public async Task Start()
        {
            if (IsRunning) { return; }

            clientManager = ClientManager.GetClientManager();
            cancellationTokenSource = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any,port);
            listener.Start();

            IsRunning = true;

            Task.Run(() => AcceptClients());

            Console.WriteLine($"server is now running on port {port}");
        }

        public void Suspend()
        {
            if (!IsRunning) { return; }

            cancellationTokenSource.Cancel();
            listener.Stop();
            clientManager.SuspandClients();

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
