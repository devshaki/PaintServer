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
    class ClientManager
    {
        private List<ClientSession> clients;
        private readonly Object listlock = new Object();
        private ClientManager() { clients = new List<ClientSession>(); }

        // singleton design pattern
        private static ClientManager clientManager;

        public static ClientManager GetClientManager()
        {
            if (clientManager == null)
            {
                clientManager = new ClientManager();
            }
            return clientManager;
        }

        public async Task AddClient(TcpClient tcpClient) 
        {
            ClientSession clientSession = new ClientSession(tcpClient);
            AddClientToList(clientSession);
            await clientSession.Start();
        }

        public async Task SuspandClients()
        {
            List<Task> tasks = new List<Task>(); 
            foreach (ClientSession client in clients)
            {
                tasks.Add(client.Stop());
            }

            await Task.WhenAll(tasks);
        }
        private void AddClientToList(ClientSession clientSession)
        {
            lock (listlock)
            {
                clients.Add(clientSession);
            }

        }


    }
}
