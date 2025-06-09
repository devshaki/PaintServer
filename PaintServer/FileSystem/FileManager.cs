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
        private FileManager() { mongoStorage = new MongoStorage(); }


        public static FileManager getFileManager()
        {
            if (fileManager == null)
            {
                fileManager = new FileManager();
            }
            return fileManager;
        }
        public string openFile(string filename,string clientId)
        {
            return mongoStorage.openFile(filename, clientId);
        }

        public void saveFile(string filename,string jsonData,string clientId) 
        {
            mongoStorage.saveFile(filename, jsonData, clientId);
        }

        public void closeFile(string filename,string clientId)
        {
            mongoStorage.closeFile(filename, clientId);
        }
    }
}
