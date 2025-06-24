﻿using System;
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
using PaintServer.MVC;
namespace PaintServer
{
    public partial class MainWindow : Window, IServerView
    {
        public event Action Suspand;
        public ObservableCollection<string> fileNames { get; } = new ObservableCollection<string>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void SuspendButtonOnClick(object sender, RoutedEventArgs e)
        {
            Suspand?.Invoke();
        }

        public void UpdateFileNames(List<string> filenames)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                fileNames.Clear();
                foreach (string filename in filenames)
                {
                    fileNames.Add(filename);
                }
            });
        }
    }
}
