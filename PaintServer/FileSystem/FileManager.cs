using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintServer.FileSystem
{
    class FileManager
    {
        private MongoStorage mongoStorage;
        private static FileManager fileManager;
        public static Action OnFilesChanged;


        private FileManager() { mongoStorage = new MongoStorage(); }


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
          if (OnFilesChanged != null)
            {
                OnFilesChanged.Invoke();
            }
        }
        public string OpenFile(string filename,string clientId)
        {
            return mongoStorage.openFile(filename, clientId);
        }

        public void SaveFile(string filename,string jsonData,string clientId) 
        {
            mongoStorage.saveFile(filename, jsonData, clientId);
            CloseFile(filename,clientId);
            UpdateFileNames();
        }

        public void CloseFile(string filename,string clientId)
        {
            mongoStorage.closeFile(filename, clientId);
        }

        public List<string> GetAllFiles()
        {
            return mongoStorage.GetAllFileNames();
        }
    }
}
