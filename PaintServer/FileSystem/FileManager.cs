using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaintServer.MVC;
using PaintServer.Server;

namespace PaintServer.FileSystem
{
    class FileManager : IServerModel
    {
        private MongoStorage mongoStorage;
        private static FileManager fileManager;
        public event Action OnFileChange;


        private FileManager() { mongoStorage = new MongoStorage(); }


        public void AddClient(ClientSession client)
        {
            mongoStorage.AddClient(client);
        }

        public List<ClientSession> GetClients()
        {
            return mongoStorage.GetAllClients();
        }
        public static FileManager GetFileManager()
        {
            if (fileManager == null)
            {
                fileManager = new FileManager();
            }
            return fileManager;
        }

        private void UpdateFileNames()
        {
            OnFileChange?.Invoke();
        }
        public string OpenFile(string filename,string clientId)
        {
            return mongoStorage.OpenFile(filename, clientId);
        }

        public void SaveFile(string filename,string jsonData,string clientId) 
        {
            mongoStorage.SaveFile(filename, jsonData, clientId);
            CloseFile(filename,clientId);
            UpdateFileNames();
        }

        public void CloseFile(string filename,string clientId)
        {
            mongoStorage.CloseFile(filename, clientId);
        }

        public List<string> GetAllFiles()
        {
            return mongoStorage.GetAllFileNames();
        }
    }
}
