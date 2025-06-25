using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;
using PaintServer.FileSystem;
using System.IO;
using PaintClient.utils;

namespace PaintServer.Server
{
    class ClientSession
    {
        private int maxHeaderSize = 100;
        private TcpClient client;
        private CancellationTokenSource cancellationTokenSource;
        public string clientId { get; private set; }
        private FileManager fileManager;
        


        public ClientSession(TcpClient client)
        {
            this.client = client;
            this.clientId = Guid.NewGuid().ToString();
            fileManager = FileManager.GetFileManager();
        }

        public async Task Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            await ReceiveMessages(cancellationTokenSource);
        }

        public void Stop()
        {
            //          client.Close();
            _ = SendHeader("suspanded:");
            cancellationTokenSource.Cancel();
        }

        public async Task ReceiveMessages(CancellationTokenSource cancellationTokenSource)
        {
            NetworkStream ns = client.GetStream();
            if (client == null || !client.Connected || ns == null) { return; };
            int bytesRead = 0;
            while (client.Connected && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                byte[] message = new byte[maxHeaderSize];
                try
                {
                    bytesRead = await ns.ReadAsync(message, 0, message.Length);
                }
                catch(IOException)
                {
                    break;
                }
                catch (SocketException)
                {
                    return;
                }
                if (cancellationTokenSource.Token.IsCancellationRequested) { return; }
                try
                {
                    string messageString = Encoding.UTF8.GetString(message, 0, bytesRead);
                    String[] metaData = messageString.Split(':');
                    System.Diagnostics.Debug.WriteLine(messageString);
                    switch (metaData[0])
                    {
                        case "upload":
                        {

                            string filename = metaData[1];
                            int filesize = int.Parse(metaData[2]);
                            await ReceiveFile(filesize, filename, cancellationTokenSource);
                                break;
                        }
                        case "get":
                        {
                            string fileName = messageString.Split(':')[1];
                            string fileJson = await GetFileJson(fileName, clientId);
                            await SendFile(fileName, fileJson, "success");
                                break;
                        }
                        case "getAll":
                        {

                            List<string> fileNames = fileManager.GetAllFiles();
                            string fileNamesJson = JsonUtils.List2Json(fileNames);
                            await SendFile("all", fileNamesJson, "success");
                                break;

                        }
                    }
                }
                catch
                {
                    Console.WriteLine($"message decode failed");
                }
                

            }
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine("suspanded");
            }

        }
        public async Task<string> GetFileJson(string fileName, string clientId)
        {
            NetworkStream ns = client.GetStream();
            if (client == null || !client.Connected || ns == null) { return ""; };
            string jsonData = fileManager.OpenFile(fileName, clientId);
            return jsonData;
        }

        public async Task SendHeader(string header)
        {
            NetworkStream ns = client.GetStream();
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            await ns.WriteAsync(headerBytes, 0, headerBytes.Length);
        }
        public async Task SendFile(string fileName,string jsonData,string headerType)
        {
            NetworkStream ns = client.GetStream();
            Console.WriteLine($"sending {jsonData}");
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
            int filesize = jsonBytes.Length;


            string header = $"{headerType}:{fileName}:{filesize}";
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            Console.WriteLine(header);
            try
            {
                await ns.WriteAsync(headerBytes, 0, headerBytes.Length);
                await ns.WriteAsync(jsonBytes, 0, filesize);
            }
            catch(IOException)
            {
                return;
            }
            catch (SocketException)
            {
                return;
            }
        }

        public async Task ReceiveFile(int fileSize, string filename,CancellationTokenSource cancellationTokenSource)
        {
            NetworkStream ns = client.GetStream();
            if (client == null || !client.Connected || ns == null) { return; };
            byte[] fileData= new byte[fileSize];
            int totalRead = 0;

            while (totalRead < fileSize)
            {
                try
                {

                    int bytesRead = await ns.ReadAsync(fileData, totalRead, fileSize - totalRead);
                    totalRead += bytesRead;
                }
                catch(IOException)
                {
                    return;
                }
                catch (SocketException)
                {
                    return;
                }
            }
            try
            {
                string jsonData = Encoding.UTF8.GetString(fileData);
                fileManager.SaveFile(filename, jsonData, clientId);
            }
            catch
            {
                Console.WriteLine($"message decode failed");
            }

           

        }


    }
}
