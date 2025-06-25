using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaintServer.FileSystem;
using PaintServer.Server;
namespace PaintServer.MVC
{
    class ServerController
    {
        private FileServer fileServer;
        private IServerView mainWindow;
        private IServerModel fileManager;

        public ServerController(IServerView mainWindow, IServerModel fileManager, FileServer fileServer)
        {
            this.fileServer = fileServer;
            this.mainWindow = mainWindow;
            this.fileManager = fileManager;
            fileManager.OnFileChange += UpdateFileNames;
            mainWindow.Suspand += OnSuspandEvent;

        }

        public void Start()
        {
            _ = fileServer.Start();
            UpdateFileNames();
        }

        public void OnSuspandEvent()
        {
            fileServer.Suspend();
        }
        public void UpdateFileNames()
        {
            List<string> names = fileManager.GetAllFiles();
            mainWindow.UpdateFileNames(names);
        }
    }
}
