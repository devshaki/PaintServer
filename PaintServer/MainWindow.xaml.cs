using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using PaintServer.Server;
using PaintServer.FileSystem;
namespace PaintServer
{
    public partial class MainWindow : Window
    {
        private FileServer fileServer;
        private ObservableCollection<string> Filenames { get; } = new ObservableCollection<string>();
        public MainWindow()
        {
            InitializeComponent();
            FileManager.OnFilesChanged = UpdateFileNames;


            fileServer = new FileServer(3333);
            fileServer.Start();
        }

        private void SuspendButtonOnClick(object sender, RoutedEventArgs e)
        {
            fileServer.Suspend();   
        }

        private void UpdateFileNames()
        {
            FileManager fileManager = FileManager.GetFileManager();
            List<string> filenames = fileManager.GetAllFiles();
            Filenames.Clear();
            foreach(string filename in filenames)
            {
                Filenames.Add(filename);
            }
        }
    }
}
