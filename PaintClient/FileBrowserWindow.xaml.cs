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
using System.Windows.Shapes;

using PaintClient.utils;
using System.Collections.ObjectModel;
using PaintClient.networking;
namespace PaintClient
{
    /// <summary>
    /// Interaction logic for FileBrowserWindow.xaml
    /// </summary>
    public partial class FileBrowserWindow : Window
    {
        public MainWindow mainWindow { get; set; }
        public ObservableCollection<string> fileNames { get; } = new ObservableCollection<string>();

        private NetworkClient networkClient;
        private readonly string ip = "127.0.0.1";
        private readonly int port = 3333;
        public FileBrowserWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            networkClient = NetworkClient.GetNetworkClient(ip, port);
            NetworkClient.RecivedFiles = null;
            NetworkClient.RecivedFiles += LoadFiles;
        }
        private void NewButtonOnClick(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            mainWindow.ClearShapes();
            this.Hide();
        }
        public void loadNames()
        {
            _ = networkClient.SendHeader("getAll", "all");
        }

        private void FilelistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainWindow.Show();
            mainWindow.ClearShapes();
            if (Filelist.SelectedItem is String fileName)
            {
               mainWindow.RequestFile(fileName);
            }
            Filelist.SelectedItem = null;
            this.Hide();
        }
        private void LoadFiles(string json)
        {

            fileNames.Clear();
            List<string> newFileNames = JsonUtils.Json2List<string>(json);
            foreach (string name in newFileNames)
            {
                fileNames.Add(name);
            }
        }
    }
}
