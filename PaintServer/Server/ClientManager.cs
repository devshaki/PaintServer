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
            Console.WriteLine($"a client has been added");
            ClientSession clientSession = new ClientSession(tcpClient);
            AddClientToList(clientSession);
            await clientSession.Start();
        }

        public void SuspandClients()
        {
            foreach (ClientSession client in clients)
            {
                client.Stop();
            }
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
