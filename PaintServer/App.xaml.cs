using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using PaintServer.FileSystem;
using PaintServer.Server;
using PaintServer.MVC;
namespace PaintServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            FileManager fileManager = FileManager.GetFileManager();
            FileServer fileServer = new FileServer(3333);

            ServerController serverController = new ServerController(mainWindow, fileManager, fileServer);

            serverController.Start();
            mainWindow.Show();
        }
    }
}
